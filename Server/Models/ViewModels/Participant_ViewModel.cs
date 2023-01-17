namespace TreasureHunt.Models;
public class Participant_ViewModel
{
  public string ParticipantId { get; set; }
  public string HuntId { get; set; }
  public string HuntTitle { get; set; }
  public string HuntOwner { get; set; }
  public string UserId { get; set; }
  public string Username { get; set; }
  public bool Accepted { get; set; }
}