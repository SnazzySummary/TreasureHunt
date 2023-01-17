using System.Collections.Generic;

namespace TreasureHunt.Models;

public class HuntObject_ViewModel
{
  public string HuntObjectId { get; set; }
  public string HuntId { get; set; }
  public int Order { get; set; }
  public string Coordinates { get; set; }
  public string Title { get; set; }
  public string Text { get; set; }
  public string Image { get; set; }
  public int Type { get; set; }
  public bool Visible { get; set; }
  public bool DefaultVisible { get; set; }
  public List<Lock_ViewModel> Locks { get; set; }
}
