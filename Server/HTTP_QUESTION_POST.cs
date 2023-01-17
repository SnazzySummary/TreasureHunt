using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using TreasureHunt.Models;
using TreasureHunt.Services;

namespace TreasureHunt.Function;
public class POST_QUESTION
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _IViewModelService;
  public POST_QUESTION(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _IViewModelService = viewModelService;
  }

  [FunctionName("Question_Post")]
  public async Task<IActionResult> PostQuestion(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "question")] HttpRequest req, ILogger log)
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
     || !form.ContainsKey("Type")
     || !form.ContainsKey("Order")
     || !form.ContainsKey("Text")
     || !form.ContainsKey("Answer")
     || !form.ContainsKey("Hint"))
    {
      return new BadRequestResult();
    }

    string LockId = form["LockId"][0];
    int Type = int.Parse(form["Type"][0]);
    int Order = int.Parse(form["Order"][0]);
    string Text = form["Text"][0];
    string Answer = form["Answer"][0];
    string Hint = form["Hint"][0];

    IActionResult result = await _databaseService.CreateQuestion(LockId, Type, Order, Text, Answer, Hint, auth.UserId);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    Question resultQuestion = resultObject.Value as Question;
    return new OkObjectResult(_IViewModelService.To_Question_ViewModel(resultQuestion));
  }
}