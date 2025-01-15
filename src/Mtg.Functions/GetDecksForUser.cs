using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mtg.Functions.Utils;

namespace Mtg.Functions;

public class GetDecksForUser(ILogger<GetDecksForUser> logger, MongoUtil mongoUtil, AuthUtil authUtil)
{
    private readonly ILogger<GetDecksForUser> _logger = logger;

    [Function("GetDecksForUser")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Deck/Mine")] HttpRequest req)
    {
        var user = await authUtil.GetUser(req.Headers);
        if (user is null)
        {
            return new UnauthorizedResult();
        }

        var result = mongoUtil.GetDecksForUser(user);
        return result.Count == 0 
            ? new NotFoundResult() 
            : new OkObjectResult(result);
    }
}