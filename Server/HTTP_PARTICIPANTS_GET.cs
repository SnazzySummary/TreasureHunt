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
public class GET_PARTICIPANTS
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _viewModelService;

  public GET_PARTICIPANTS(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _viewModelService = viewModelService;
  }

  [FunctionName("Participants_Get")]
  public async Task<IActionResult> GetParticipants(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "participants")] HttpRequest req, ILogger log)
  {
    // Check if we have authentication info.
    ValidateJWT auth = new ValidateJWT(req);
    if (!auth.IsValid)
    {
      return new UnauthorizedResult(); // No authentication info.
    }

    IActionResult result = await _databaseService.GetUsersParticipants(auth.UserId);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    List<Participant> participants = resultObject.Value as List<Participant>;
    return new OkObjectResult(await _viewModelService.To_Participant_ViewModels(participants));
  }
}