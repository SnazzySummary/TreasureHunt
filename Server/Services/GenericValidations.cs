using System;
using System.Text.RegularExpressions;

namespace TreasureHunt.Services;
/* Performs simple validation checks */
public static class GenericValidations
{
  /* Returns true if the string parameter contains only the allowed characters */
  public static bool IsValidCharacters(string text)
  {
    string pat = "^[ A-Za-z0-9_@,.+-]*$";
    var match = Regex.Match(text, pat, RegexOptions.IgnoreCase);
    return match.Success;
  }

  /* Returns true if the string is a valid Guid */
  public static bool IsValidGUID(string text)
  {
    return Guid.TryParse(text, out _);
  }
}