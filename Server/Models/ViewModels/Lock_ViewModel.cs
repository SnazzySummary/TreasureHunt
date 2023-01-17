using System.Collections.Generic;

namespace TreasureHunt.Models;

public class Lock_ViewModel
{
  public string LockId { get; set; }
  public string HuntObjectId { get; set; }
  public int Type { get; set; }
  public int Order { get; set; }
  public bool Locked { get; set; }

  public List<UnlockAction_ViewModel> UnlockActions { get; set; }

  public List<Question_ViewModel> Questions { get; set; }
}
