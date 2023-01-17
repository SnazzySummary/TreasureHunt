using System.Collections.Generic;

namespace TreasureHunt.Models;

public class Hunt_ViewModel
{
  public string HuntId { get; set; }

  public string UserId { get; set; }

  public string Username { get; set; }

  public string Title { get; set; }

  public List<Participant_ViewModel> Participants { get; set; }

  // Possibly store as VARBINARY(max)
  public string Image { get; set; }

}
