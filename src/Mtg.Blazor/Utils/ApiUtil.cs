using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.SessionStorage;
using Mtg.Blazor.Exceptions;
using Mtg.Blazor.Models;

namespace Mtg.Blazor.Utils;

public class ApiUtil
{
    /// <summary>
    /// JSON serialization options to use throughout this util.
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Lets us get a JWT for the user.
    /// </summary>
    private readonly TokenUtil _tokenUtil;
    
    /// <summary>
    /// Lets us access session storage.
    /// </summary>
    private readonly ISessionStorageService _storageService;
    
    /// <summary>
    /// An HTTP client.
    /// </summary>
    private readonly HttpClient _http;

    /// <summary>
    /// URL for this environment's Azure fuctions.
    /// </summary>
    private readonly string _functionsUri; 

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="tokenUtil"></param>
    /// <param name="storageService"></param>
    /// <param name="http"></param>
    /// <param name="config"></param>
    /// <exception cref="DataException"></exception>
    public ApiUtil(TokenUtil tokenUtil, ISessionStorageService storageService, HttpClient http, IConfiguration config)
    {
        _tokenUtil = tokenUtil;
        _storageService = storageService;
        _http = http;

        var uri = config.GetSection("FunctionsUri").Value
            ?? throw new DataException("Could not find FunctionUri configuration section.");
        _functionsUri = uri;
    }

    /// <summary>
    /// Gets all decks.
    /// </summary>
    /// <returns>A list of all decks.</returns>
    public async Task<List<Deck>> GetDecks()
    {
        var decks = await CallApi<List<Deck>>(HttpMethod.Get, "Deck");
        return decks;
    }

    /// <summary>
    /// Create a deck.
    /// </summary>
    /// <param name="newDeck">The deck to create.</param>
    public async Task CreateDeck(Deck newDeck)
    {
        await CallApiVoidWithParam(HttpMethod.Post, "Deck", new CreateDeckRequest
        {
            NewDeck = newDeck
        });
    }

    /// <summary>
    /// Gets a single deck.
    /// </summary>
    /// <param name="deckId">The ID of the deck.</param>
    /// <returns>A deck.</returns>
    public async Task<Deck> GetDeck(Guid deckId)
    {
        var deck = await CallApiWithParam<Deck, Guid>(HttpMethod.Get, "Deck", deckId);
        return deck;
    }

    /// <summary>
    /// Gets all decks a user has created.
    /// </summary>
    /// <returns>A list of decks.</returns>
    public async Task<List<Deck>> GetDecksForUser()
    {
        var decks = await CallApi<List<Deck>>(HttpMethod.Get, "Deck/Mine");
        return decks;
    }

    /// <summary>
    /// Searches our database for decks that match the query.
    /// </summary>
    /// <param name="query">The query from the user.</param>
    /// <returns>A list of decks that match.</returns>
    public async Task<List<Deck>> SearchDecks(string query)
    {
        var decks = await CallApi<List<Deck>>(HttpMethod.Get, $"Search?query={query}");
        return decks;
    }

    public async Task CreateMatch(Match match)
    {
        await CallApiVoidWithParam<CreateMatchRequest>(HttpMethod.Post, "Match", new CreateMatchRequest
        {
            NewMatch = match
        });
    }

    #region SCRYFALL
    
    /// <summary>
    /// Gets a random funny card from Scryfall.
    /// </summary>
    /// <returns>An url to an image.</returns>
    public async Task<string> GetRandomFunnyCard()
    {
        var response = await _http.GetAsync("https://api.scryfall.com/cards/random?q=is:funny+-set:da1");
        var jsonString = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<ScryFallCard>(jsonString, _jsonOptions) 
                  ?? throw new ArgumentException("Could not parse response from ScryFall api.");
        return obj.ImageUris.Png;
    }

    /// <summary>
    /// Gets commanders that match the user's requested query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>A list of possible commanders that match the requested query.</returns>
    public async Task<List<ScryFallCard>> GetCommanders(string query)
    {
        var response = await _http.GetAsync($"https://api.scryfall.com/cards/search?q=is:commander+name:{query}");
        var jsonString = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<ScryFallList>(jsonString, _jsonOptions)
                ?? throw new ArgumentException("Could not parse response from ScryFall api.");
        return obj.Data;
    }
    
    #endregion

    #region PRIVATE
    
    private async Task<TReturn> CallApiWithParam<TReturn,TParam>(HttpMethod method, string uri, TParam param)
    {
        var response = await CallApiBaseWithParam(method, uri, param);
        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.NoContent)
        {
            throw new ApiException("");
        }
        
        var responseStr = await response.Content.ReadAsStringAsync();
        var responseObj = JsonSerializer.Deserialize<TReturn>(responseStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (responseObj is null)
        {
            throw new Exception();
        }
        
        return responseObj;
    }

    private async Task CallApiVoidWithParam<TParam>(HttpMethod method, string uri, TParam parameters)
    {
        var response = await CallApiBaseWithParam(method, uri, parameters);
    }

    private async Task<HttpResponseMessage> CallApiBaseWithParam<TParam>(HttpMethod method, string uri, TParam parameters)
    {
        if (_tokenUtil.Jwt == string.Empty)
        {
            await _tokenUtil.GetJwt(_storageService);
        }

        var req = CreateBaseRequestWithParam(method, uri, parameters);
        var res = await _http.SendAsync(req);
        return res;
    }
    
    private HttpRequestMessage CreateBaseRequestWithParam<TParam>(HttpMethod method, string uri, TParam parameters, bool authenticated=true)
    {
        var request = new HttpRequestMessage(method, $"{_functionsUri}{uri}");
        request.Headers.TryAddWithoutValidation("accept", "*/*");

        if (authenticated)
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_tokenUtil.Jwt}");
        }

        request.Content = new StringContent(JsonSerializer.Serialize(parameters));
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        
        return request;
    }
    
    private async Task<TReturn> CallApi<TReturn>(HttpMethod method, string uri)
    {
        var response = await CallApiBase(method, uri);
        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.NoContent)
        {
            throw new ApiException("");
        }
        
        var responseStr = await response.Content.ReadAsStringAsync();
        var responseObj = JsonSerializer.Deserialize<TReturn>(responseStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (responseObj is null)
        {
            throw new Exception();
        }
        
        return responseObj;
    }

    private async Task CallApiVoid(HttpMethod method, string uri)
    {
        var response = await CallApiBase(method, uri);
    }

    private async Task<HttpResponseMessage> CallApiBase(HttpMethod method, string uri)
    {
        if (_tokenUtil.Jwt == string.Empty)
        {
            await _tokenUtil.GetJwt(_storageService);
        }

        var req = CreateBaseRequest(method, uri);
        var res = await _http.SendAsync(req);
        return res;
    }
    
    private HttpRequestMessage CreateBaseRequest(HttpMethod method, string uri, bool authenticated=true)
    {
        var request = new HttpRequestMessage(method, $"{_functionsUri}{uri}");
        request.Headers.TryAddWithoutValidation("accept", "*/*");

        if (authenticated)
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_tokenUtil.Jwt}");
        }

        return request;
    }
    
    #endregion
}