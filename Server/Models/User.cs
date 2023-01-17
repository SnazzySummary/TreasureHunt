using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Primitives;

namespace TreasureHunt.Models;
public class User
{
  [Key]
  public string UserId { get; set; }

  [Required]
  public string Username { get; set; }

  [Required]
  public string Password { get; set; }

  [Required]
  public string Email { get; set; }

  [Required]
  public string FirstName { get; set; }

  [Required]
  public string LastName { get; set; }

  public ICollection<Hunt> Hunts { get; set; }

  public ICollection<Participant> Participants { get; set; }

  //Possibly store as VARBINARY(max)
  // public Image Thumbnail { get; set; }
  public User()
  {
    this.Hunts = new List<Hunt>();
    this.Participants = new List<Participant>();
  }
}