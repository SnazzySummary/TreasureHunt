using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Reflection;

namespace TreasureHunt.Models;

public class Hunt
{
  [Key]
  public string HuntId { get; set; }

  [Required]
  public string UserId { get; set; }

  public User User { get; set; }

  [Required]
  public string Title { get; set; }

  public ICollection<HuntObject> HuntObjects { get; set; }

  public ICollection<Participant> Participants { get; set; }
  //Possibly store as VARBINARY(max)
  // public Image Thumbnail { get; set; }

  public Hunt()
  {
    this.HuntObjects = new List<HuntObject>();
    this.Participants = new List<Participant>();
  }
}
