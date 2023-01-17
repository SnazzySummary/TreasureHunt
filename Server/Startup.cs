using System;
using TreasureHunt.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TreasureHunt.Data;
using TreasureHunt.Services;

[assembly: FunctionsStartup(typeof(TreasureHunt.StartUp))]
namespace TreasureHunt
{
  public class StartUp : FunctionsStartup
  {
    public override void Configure(IFunctionsHostBuilder builder)
    {
      builder.Services.AddDbContext<TreasureHuntContext>(options =>
          options.UseSqlServer(Helper.GetSetting("SQL-SERVER_DATABASE_CONNECTION_STRING")));
      builder.Services.AddScoped<IDatabaseService, DatabaseService>();
      builder.Services.AddScoped<IViewModelService, ViewModelService>();
    }
  }
}
