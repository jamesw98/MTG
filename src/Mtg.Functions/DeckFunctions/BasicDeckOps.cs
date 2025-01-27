using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mtg.Functions.Models;
using Mtg.Functions.Utils;

namespace Mtg.Functions;

public class BasicDeckOps(ILogger<BasicDeckOps> logger, MongoUtil mongoUtil, AuthUtil authUtil)
{
    private readonly ILogger<BasicDeckOps> _logger = logger;

    [Function("Deck")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, [FromBody] Deck newDeck)
    {
        var user = await authUtil.GetUser(req.Headers);
        if (user is null)
        {
            return new UnauthorizedResult();
        }
        
        if (req.Method == HttpMethods.Post)
        {
            mongoUtil.CreateDeck(newDeck, user);
            return new NoContentResult();
        }
        
        if (req.Method == HttpMethods.Get)
        {
            return new OkObjectResult(mongoUtil.GetDecks());
        }

        return new NotFoundResult();
    }
}