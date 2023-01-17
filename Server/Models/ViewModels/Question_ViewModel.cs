using System.Collections.Generic;

namespace TreasureHunt.Models;

public class Question_ViewModel
{
  public string QuestionId { get; set; }
  public string LockId { get; set; }
  public int Type { get; set; }
  public int Order { get; set; }
  public bool Answered { get; set; }
  public string Text { get; set; }
  public string Answer { get; set; }
  public string Hint { get; set; }
}
