using System.ComponentModel.DataAnnotations;

namespace TreasureHunt.Models;
public class Question
{
  [Key]
  public string QuestionId { get; set; }

  [Required]
  public string LockId { get; set; }

  public Lock Lock { get; set; }

  [Required]
  public int Type { get; set; }

  [Required]
  public int Order { get; set; }

  [Required]
  public bool Answered { get; set; }

  [Required]
  public string Text { get; set; }

  [Required]
  public string Answer { get; set; }

  public string Hint { get; set; }
}