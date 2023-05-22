using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TreasureHunt.Models;
using TreasureHunt.Services;

namespace TreasureHunt.Function;
public class GET_HUNTS
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _viewModelService;
  //todo need filesystemservice?

  public GET_HUNTS(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _viewModelService = viewModelService;
  }

  [FunctionName("Hunts_Get")]
  public async Task<IActionResult> GetHunts(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "hunts")] HttpRequest req, ILogger log)
  {
    // Check if we have authentication info.
    ValidateJWT auth = new ValidateJWT(req);
    if (!auth.IsValid)
    {
      return new UnauthorizedResult(); // No authentication info.
    }

    IActionResult result = await _databaseService.GetUsersHunts(auth.Username);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    List<Hunt> hunts = resultObject.Value as List<Hunt>;
    return new OkObjectResult(await _viewModelService.To_Hunt_ViewModels(hunts));
  }
}