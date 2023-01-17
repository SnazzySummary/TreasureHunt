using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TreasureHunt.Models;
using TreasureHunt.Services;

namespace TreasureHunt.Function;
public class POST_UNLOCK_ACTION
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _IViewModelService;
  public POST_UNLOCK_ACTION(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _IViewModelService = viewModelService;
  }

  [FunctionName("Unlock_Action_Post")]
  public async Task<IActionResult> PostUnlockAction(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "unlockaction")] HttpRequest req, ILogger log)
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
    if (!form.ContainsKey("LockId")
     || !form.ContainsKey("HuntObjectId"))
    {
      return new BadRequestResult();
    }

    string LockId = form["LockId"][0];
    string HuntObjectId = form["HuntObjectId"][0];

    IActionResult result = await _databaseService.CreateUnlockAction(LockId, HuntObjectId, auth.UserId);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    UnlockAction resultUnlockAction = resultObject.Value as UnlockAction;
    return new OkObjectResult(_IViewModelService.To_UnlockAction_ViewModel(resultUnlockAction));
  }
}