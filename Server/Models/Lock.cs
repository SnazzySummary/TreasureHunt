using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TreasureHunt.Models;
public class Lock
{
  [Key]
  public string LockId { get; set; }

  [Required]
  public string HuntObjectId { get; set; }

  public HuntObject HuntObject { get; set; }

  [Required]
  public int Type { get; set; }

  [Required]
  public int Order { get; set; }

  [Required]
  public bool Locked { get; set; }

  public ICollection<UnlockAction> UnlockActions { get; set; }

  public ICollection<Question> Questions { get; set; }

  public Lock()
  {
    this.UnlockActions = new List<UnlockAction>();
    this.Questions = new List<Question>();
  }
}