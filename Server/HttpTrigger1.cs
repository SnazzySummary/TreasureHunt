using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TreasureHunt.Data;
using System.Linq;
using TreasureHunt.Services;

namespace TreasureHunt.Function;
public class HttpTrigger1
{
  private readonly IDatabaseService _databaseService;
  // private readonly TreasureHuntContext _context;

  public HttpTrigger1(IDatabaseService databaseService)
  {
    _databaseService = databaseService;
  }

  [FunctionName("HttpTrigger1")]
  public Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
      ILogger log)
  {
    log.LogInformation("C# HTTP trigger function processed a request.");
    // string name = req.Query["name"];
    // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    // dynamic data = JsonConvert.DeserializeObject(requestBody);

    return _databaseService.GetAllDataForTesting();
  }
}