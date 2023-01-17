using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
// using CsvHelper;
// using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;

namespace TreasureHunt.Data;

public class Helper
{
  public static string GetSetting(string name)
  {
    IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    string connStr = Configuration[$"Values:{name}"];

    if (string.IsNullOrEmpty(connStr))
      connStr = Environment.GetEnvironmentVariable(name);

    return connStr;
  }
}