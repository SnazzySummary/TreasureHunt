using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using TreasureHunt.Services;

namespace TreasureHunt.Function;
public class DELETE_HUNT
{
  private readonly IDatabaseService _databaseService;
  public DELETE_HUNT(IDatabaseService databaseService)
  {
    _databaseService = databaseService;
  }

  [FunctionName("Hunt_Delete")]
  public async Task<IActionResult> DeleteHunt(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "hunt")] HttpRequest req, ILogger log)
  {
    // Check if we have authentication info.
    ValidateJWT auth = new ValidateJWT(req);
    if (!auth.IsValid)
    {
      return new UnauthorizedResult(); // No authentication info.
    }

    // Check if the request has form data
    if (!req.HasFormContentType)
    {
      return new BadRequestResult();
    }

    // Checks if the form has all the required info and gets it all.
    dynamic form = req.Form;
    if (!form.ContainsKey("HuntId"))
    {
      return new BadRequestResult();
    }
    string HuntId = form["HuntId"][0];

    return await _databaseService.DeleteHunt(HuntId, auth.UserId); ;
  }
}