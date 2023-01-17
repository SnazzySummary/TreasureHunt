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
public class EDIT_LOCK
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _IViewModelService;
  public EDIT_LOCK(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _IViewModelService = viewModelService;
  }

  [FunctionName("Lock_Edit")]
  public async Task<IActionResult> PutLock(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lock")] HttpRequest req, ILogger log)
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
    if (!form.ContainsKey("LockId") || !form.ContainsKey("Type") || !form.ContainsKey("Order"))
    {
      return new BadRequestResult();
    }

    string LockId = form["LockId"][0];
    int Type = int.Parse(form["Type"][0]);
    int Order = int.Parse(form["Order"][0]);


    IActionResult result = await _databaseService.EditLock(LockId, Type, Order, auth.UserId);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    Lock resultLock = resultObject.Value as Lock;
    return new OkObjectResult(_IViewModelService.To_Lock_ViewModel(resultLock));
  }
}