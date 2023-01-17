using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TreasureHunt.Models;
using TreasureHunt.Services;


namespace TreasureHunt.Function;
public class Function1
{
  private readonly IDatabaseService _databaseService;

  public Function1(IDatabaseService databaseService)
  {
    _databaseService = databaseService;
  }

  [FunctionName("UserAuthenication")]
  public async Task<IActionResult> UserAuthenication(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")] UserCredentials userCredentials, ILogger log)
  {
    log.LogInformation("C# HTTP trigger function processed a request.");

    //TODO: Get username -> ensure it's valid -> throw error if not
    if (!GenericValidations.IsValidCharacters(userCredentials.User))
    {
      return await Task.FromResult(new UnauthorizedResult()).ConfigureAwait(false);
    }

    //TODO: authenticated = DatabaseService.Login(username, password)
    User authenticatedUser = await _databaseService.Login(userCredentials.User, userCredentials.Password);
    if (authenticatedUser == null)
    {
      return await Task.FromResult(new UnauthorizedResult()).ConfigureAwait(false);
    }
    // bool authenticated = userCredentials?.User.Equals("Jay", StringComparison.InvariantCultureIgnoreCase) ?? false;
    // if (!authenticated)
    // {
    //   return await Task.FromResult(new UnauthorizedResult()).ConfigureAwait(false);
    // }
    else
    {
      GenerateJWTToken generateJWTToken = new();
      string token = generateJWTToken.IssuingJWT(authenticatedUser);
      return await Task.FromResult(new OkObjectResult(token)).ConfigureAwait(false);
    }
  }
}
public class UserCredentials
{
  public string User
  {
    get;
    set;
  }
  public string Password
  {
    get;
    set;
  }
}



// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Azure.WebJobs;
// using Microsoft.Azure.WebJobs.Extensions.Http;
// using Microsoft.Extensions.Logging;
// using System;
// using System.Threading.Tasks;
// namespace TreasureHunt.Function;

// public static class UserAuthentication
// {
//   [FunctionName("UserAuthenication")]
//   public static async Task<IActionResult> Run(
//       [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")] UserCredentials userCredentials, ILogger log)
//   {
//     log.LogInformation("C# HTTP trigger function processed a request.");
//     // TODO: Perform custom authentication here; we're just using a simple hard coded check for this example
//     bool authenticated = userCredentials?.User.Equals("Jay", StringComparison.InvariantCultureIgnoreCase) ?? false;
//     if (!authenticated)
//     {
//       return await Task.FromResult(new UnauthorizedResult()).ConfigureAwait(false);
//     }
//     else
//     {
//       return await Task.FromResult(new OkObjectResult("User is Authenticated")).ConfigureAwait(false);
//     }
//   }
// }

// public class UserCredentials
// {
//   public string User
//   {
//     get;
//     set;
//   }
//   public string Password
//   {
//     get;
//     set;
//   }
// }