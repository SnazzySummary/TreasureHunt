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
public class POST_HUNT_OBJECT
{
  private readonly IDatabaseService _databaseService;
  private readonly IViewModelService _IViewModelService;
  public POST_HUNT_OBJECT(IDatabaseService databaseService, IViewModelService viewModelService)
  {
    _databaseService = databaseService;
    _IViewModelService = viewModelService;
  }

  [FunctionName("Hunt_Object_Post")]
  public async Task<IActionResult> PostHuntObject(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "huntobject")] HttpRequest req, ILogger log)
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
     || !form.ContainsKey("Order")
     || !form.ContainsKey("Coordinates")
     || !form.ContainsKey("Title")
     || !form.ContainsKey("Text")
     || !form.ContainsKey("Type")
     || !form.ContainsKey("DefaultVisible"))
    {
      return new BadRequestResult();
    }

    string HuntId = form["HuntId"][0];
    int Order = int.Parse(form["Order"][0]);
    string Coordinates = form["Coordinates"][0];
    string Title = form["Title"][0];
    string Text = form["Text"][0];
    int Type = int.Parse(form["Type"][0]);
    bool DefaultVisible = false;
    if (form["DefaultVisible"][0].Equals("true"))
    {
      DefaultVisible = true;
    }

    IActionResult result = await _databaseService.CreateHuntObject(HuntId, Order, Coordinates, Title, Text, Type, DefaultVisible, auth.UserId, req);
    if (!result.GetType().Equals(typeof(OkObjectResult)))
    {
      return result;
    }
    OkObjectResult resultObject = result as OkObjectResult;
    HuntObject resultHuntObject = resultObject.Value as HuntObject;
    return new OkObjectResult(_IViewModelService.To_HuntObject_ViewModel(resultHuntObject));
  }
}