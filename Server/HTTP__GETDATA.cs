using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TreasureHunt.Services;

namespace TreasureHunt.Function;
public static class Function2
{
  [FunctionName("GetData")]
  public static async Task<IActionResult> GetData(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "data")] HttpRequest req, ILogger log)
  {
    // Check if we have authentication info.
    ValidateJWT auth = new ValidateJWT(req);
    if (!auth.IsValid)
    {
      return new UnauthorizedResult(); // No authentication info.
    }
    string postData = auth.Username;
    return new OkObjectResult(postData);
  }
}