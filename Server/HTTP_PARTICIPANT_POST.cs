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
public class POST_PARTICIPANT
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _IViewModelService;
  public POST_PARTICIPANT(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _IViewModelService = viewModelService;
  }

  [FunctionName("Participant_Post")]
  public async Task<IActionResult> PostParticipant(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "participant")] HttpRequest req, ILogger log)
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
    if (!form.ContainsKey("HuntId")
     || !form.ContainsKey("InvitedUserId"))
    {
      return new BadRequestResult();
    }

    string HuntId = form["HuntId"][0];
    string InvitedUserId = form["InvitedUserId"][0];

    IActionResult result = await _databaseService.CreateParticipant(HuntId, InvitedUserId, auth.UserId);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    Participant resultParticipant = resultObject.Value as Participant;
    return new OkObjectResult(_IViewModelService.To_Participant_ViewModel(resultParticipant));
  }
}