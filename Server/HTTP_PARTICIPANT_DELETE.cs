using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using TreasureHunt.Services;

namespace TreasureHunt.Function;
public class DELETE_PARTICIPANT
{
  private readonly IDatabaseService _databaseService;
  public DELETE_PARTICIPANT(IDatabaseService databaseService)
  {
    _databaseService = databaseService;
  }

  [FunctionName("Participant_Delete")]
  public async Task<IActionResult> DeleteParticipant(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "participant")] HttpRequest req, ILogger log)
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

    return await _databaseService.DeleteParticipant(ParticipantId, auth.UserId);
  }
}