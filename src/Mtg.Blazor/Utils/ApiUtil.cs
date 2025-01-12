using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.SessionStorage;
using Mtg.Blazor.Exceptions;
using Mtg.Blazor.Models;

namespace Mtg.Blazor.Utils;

public class ApiUtil(TokenUtil tokenUtil, ISessionStorageService storageService, HttpClient http)
{
    public async Task<List<Deck>> GetDecks()
    {
        var decks = await CallApi<List<Deck>>(HttpMethod.Get, "Deck");
        return decks;
    }

    public async Task CreateDeck(Deck newDeck)
    {
        await CallApiVoidWithParam(HttpMethod.Post, "Deck", new CreateDeckRequest{NewDeck = newDeck});
    }

    public async Task<Deck> GetDeck(Guid deckId)
    {
        var deck = await CallApiWithParam<Deck, Guid>(HttpMethod.Get, "Deck", deckId);
        return deck;
    }

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
        if (tokenUtil.Jwt == string.Empty)
        {
            await tokenUtil.GetJwt(storageService);
        }

        var req = CreateBaseRequestWithParam(method, uri, parameters);
        var res = await http.SendAsync(req);
        return res;
    }
    
    private HttpRequestMessage CreateBaseRequestWithParam<TParam>(HttpMethod method, string uri, TParam parameters, bool authenticated=true)
    {
        var request = new HttpRequestMessage(method, $"http://localhost:7047/{uri}");
        request.Headers.TryAddWithoutValidation("accept", "*/*");

        if (authenticated)
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {tokenUtil.Jwt}");
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
        if (tokenUtil.Jwt == string.Empty)
        {
            await tokenUtil.GetJwt(storageService);
        }

        var req = CreateBaseRequest(method, uri);
        var res = await http.SendAsync(req);
        return res;
    }
    
    private HttpRequestMessage CreateBaseRequest(HttpMethod method, string uri, bool authenticated=true)
    {
        var request = new HttpRequestMessage(method, $"http://localhost:7047/{uri}");
        request.Headers.TryAddWithoutValidation("accept", "*/*");

        if (authenticated)
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {tokenUtil.Jwt}");
        }

        return request;
    }
    
    #endregion
}