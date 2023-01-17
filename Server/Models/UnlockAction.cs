using System.ComponentModel.DataAnnotations;

namespace TreasureHunt.Models;
public class UnlockAction
{
  [Key]
  public string UnlockActionId { get; set; }

  [Required]
  public string LockId { get; set; }

  public Lock Lock { get; set; }

  [Required]
  public string HuntObjectId { get; set; }

  public HuntObject HuntObject { get; set; }
}