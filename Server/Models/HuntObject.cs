using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace TreasureHunt.Models;
public class HuntObject
{
  [Key]
  public string HuntObjectId { get; set; }

  [Required]
  public string HuntId { get; set; }

  public Hunt Hunt { get; set; }

  [Required]
  public int Order { get; set; }

  public string Coordinates { get; set; }

  public string Title { get; set; }
  public string Text { get; set; }

  // public string Title
  // {
  //   get { return Title; }
  //   set
  //   {
  //     // Do validation -> Throw error of invalid
  //     Title = value;
  //   }
  // }

  // public string Text
  // {
  //   get { return Text; }
  //   set
  //   {
  //     // Do validation -> Throw error of invalid
  //     Title = value;
  //   }
  // }

  // public Image Image { get; set; }

  [Required]
  public int Type { get; set; }

  [Required]
  public bool Visible { get; set; }

  [Required]
  public bool DefaultVisible { get; set; }

  public ICollection<Lock> Locks { get; set; }

  public ICollection<UnlockAction> UnlockActions { get; set; }

  public HuntObject()
  {
    this.Locks = new List<Lock>();
    this.UnlockActions = new List<UnlockAction>();
  }
}