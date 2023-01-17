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
public class POST_LOCK
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _IViewModelService;
  public POST_LOCK(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _IViewModelService = viewModelService;
  }

  [FunctionName("Lock_Post")]
  public async Task<IActionResult> PostLock(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "lock")] HttpRequest req, ILogger log)
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
    if (!form.ContainsKey("HuntObjectId")
     || !form.ContainsKey("Type")
     || !form.ContainsKey("Order"))
    {
      return new BadRequestResult();
    }
    string HuntObjectId = form["HuntObjectId"][0];
    int Type = int.Parse(form["Type"][0]);
    int Order = int.Parse(form["Order"][0]);

    IActionResult result = await _databaseService.CreateLock(HuntObjectId, Type, Order, auth.UserId);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    Lock resultLock = resultObject.Value as Lock;
    return new OkObjectResult(_IViewModelService.To_Lock_ViewModel(resultLock));
  }
}