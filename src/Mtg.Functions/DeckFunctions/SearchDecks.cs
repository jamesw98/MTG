using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mtg.Functions.Utils;

namespace Mtg.Functions;

public class SearchDecks(ILogger<SearchDecks> logger, AuthUtil authUtil, MongoUtil mongoUtil)
{
    [Function("SearchDecks")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "Search")] HttpRequest req, [FromQuery] string query)
    {
        var user = await authUtil.GetUser(req.Headers);
        if (user is null)
        {
            return new UnauthorizedResult();
        }

        var decks = mongoUtil.SearchDecks(query);
        return new OkObjectResult(decks);
    }

}