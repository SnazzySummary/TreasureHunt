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
public class ACCEPT_PARTICIPANT
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _IViewModelService;
  public ACCEPT_PARTICIPANT(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _IViewModelService = viewModelService;
  }

  [FunctionName("Participant_Accept")]
  public async Task<IActionResult> AcceptParticipant(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "participant")] HttpRequest req, ILogger log)
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
    if (!form.ContainsKey("ParticipantId"))
    {
      return new BadRequestResult();
    }

    string ParticipantId = form["ParticipantId"][0];

    IActionResult result = await _databaseService.AcceptParticipant(ParticipantId, auth.UserId);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    Participant resultParticipant = resultObject.Value as Participant;
    return new OkObjectResult(_IViewModelService.To_Participant_ViewModel(resultParticipant));
  }
}