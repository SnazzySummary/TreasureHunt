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
public class POST_HUNT
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _IViewModelService;
  public POST_HUNT(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _IViewModelService = viewModelService;
  }

  [FunctionName("Hunt_Post")]
  public async Task<IActionResult> PostHunt(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "hunt")] HttpRequest req, ILogger log)
  {
    // Check if we have authentication info.
    ValidateJWT auth = new ValidateJWT(req);
    if (!auth.IsValid)
    {
      return new UnauthorizedResult(); // No authentication info.
    }

    // Checks if the form has all the required info and gets it all.
    dynamic form = req.Form;
    if (!form.ContainsKey("Title"))
    {
      return new BadRequestResult();
    }

    string title = form["Title"][0];

    IActionResult result = await _databaseService.CreateHunt(auth.UserId, title, req);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    Hunt resultHunt = resultObject.Value as Hunt;

    return new OkObjectResult(await _IViewModelService.To_Hunt_ViewModel(resultHunt));
  }
}