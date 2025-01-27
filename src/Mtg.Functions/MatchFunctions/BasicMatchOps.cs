using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mtg.Functions.Models;
using Mtg.Functions.Utils;

namespace Mtg.Functions.MatchFunctions;

public class BasicMatchOps(ILogger<BasicDeckOps> logger, MongoUtil mongoUtil, AuthUtil authUtil)
{

    [Function("Match")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Match")] HttpRequest req, [FromBody] Match newMatch)
    { 
        var user = await authUtil.GetUser(req.Headers);
        if (user is null)
        {
            return new UnauthorizedResult();
        }
        
        if (req.Method == HttpMethods.Post)
        {
            mongoUtil.CreateMatch(newMatch);
            return new NoContentResult();
        }
        
        if (req.Method == HttpMethods.Get)
        {
            return new OkObjectResult(mongoUtil.GetMatches());
        }

        return new NotFoundResult();
    }

}