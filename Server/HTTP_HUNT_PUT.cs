using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using TreasureHunt.Models;
using TreasureHunt.Services;

namespace TreasureHunt.Function;
public class EDIT_HUNT
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _IViewModelService;
  public EDIT_HUNT(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _IViewModelService = viewModelService;
  }

  [FunctionName("Hunt_Edit")]
  public async Task<IActionResult> PutHunt(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "hunt")] HttpRequest req, ILogger log)
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
    if (!form.ContainsKey("HuntId") || !form.ContainsKey("Title"))
    {
      return new BadRequestResult();
    }
    string HuntId = form["HuntId"][0];
    string Title = form["Title"][0];

    IActionResult result = await _databaseService.EditHunt(auth.UserId, Title, HuntId, req);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    Hunt resultHunt = resultObject.Value as Hunt;
    return new OkObjectResult(_IViewModelService.To_Hunt_ViewModel(resultHunt));
  }
}