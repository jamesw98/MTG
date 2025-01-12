using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mtg.Functions.Utils;

namespace Mtg.Functions;

public class GetSingleDeck(ILogger<BasicDeckOps> logger, MongoUtil mongoUtil, AuthUtil authUtil)
{
    private readonly ILogger<BasicDeckOps> _logger = logger;

    [Function("GetSingleDeck")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Deck/{guid}")] HttpRequest req, Guid guid)
    {
        var user = await authUtil.GetUser(req.Headers);
        if (user is null)
        {
            return new UnauthorizedResult();
        }
        
        var deck = mongoUtil.GetDeck(guid);
        return deck is null 
            ? new NotFoundResult() 
            : new OkObjectResult(deck);
    }
}