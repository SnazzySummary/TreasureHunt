using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TreasureHunt.Models;
using TreasureHunt.Services;

namespace TreasureHunt.Function;
public class GET_HUNT_OBJECTS
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _viewModelService;

  public GET_HUNT_OBJECTS(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _viewModelService = viewModelService;
  }

  [FunctionName("Hunt_Objects_Get")]
  public async Task<IActionResult> GetHuntObjects(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "huntobjects")] HttpRequest req, ILogger log)
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

    // Uses the form data for the request
    IActionResult result = await _databaseService.GetHuntsObjects(HuntId, auth.UserId);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    List<HuntObject> huntObjects = resultObject.Value as List<HuntObject>;
    return new OkObjectResult(_viewModelService.To_HuntObject_ViewModels(huntObjects));
  }
}