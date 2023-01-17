using System.ComponentModel.DataAnnotations;

namespace TreasureHunt.Models;
public class Participant
{
  [Key]
  public string ParticipantId { get; set; }
  [Required]
  public string HuntId { get; set; }

  public Hunt Hunt { get; set; }
  [Required]
  public string UserId { get; set; }
  public User User { get; set; }
  [Required]
  public bool Accepted { get; set; }
}