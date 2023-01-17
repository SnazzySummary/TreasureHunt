using System;
using System.Collections.Generic;
using TreasureHunt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using TreasureHunt.Models;

namespace TreasureHunt.Data
{
  public partial class TreasureHuntContext : DbContext
  {
    public TreasureHuntContext() { }

    public TreasureHuntContext(DbContextOptions<TreasureHuntContext> options) : base(options) { }

    public TreasureHuntContext(DbContextOptions options) : base(options) { }


    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UnlockAction> UnlockActions { get; set; }
    public virtual DbSet<Question> Questions { get; set; }
    public virtual DbSet<Participant> Participants { get; set; }
    public virtual DbSet<Lock> Locks { get; set; }
    public virtual DbSet<HuntObject> HuntObjects { get; set; }
    public virtual DbSet<Hunt> Hunts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      //User
      builder.Entity<User>()
        .HasKey(u1 => u1.UserId);

      //Hunt
      builder.Entity<Hunt>()
        .HasKey(h1 => h1.HuntId);

      builder.Entity<Hunt>()
        .HasOne(h1 => h1.User)
        .WithMany(h => h.Hunts)
        .HasForeignKey(h1 => h1.UserId);

      //HuntObject
      builder.Entity<HuntObject>()
        .HasKey(h1 => h1.HuntObjectId);

      builder.Entity<HuntObject>()
        .HasOne(h1 => h1.Hunt)
        .WithMany(h => h.HuntObjects)
        .HasForeignKey(h1 => h1.HuntId);

      //Lock
      builder.Entity<Lock>()
        .HasKey(l1 => l1.LockId);

      builder.Entity<Lock>()
        .HasOne(l1 => l1.HuntObject)
        .WithMany(h => h.Locks)
        .HasForeignKey(l1 => l1.HuntObjectId);

      //Participant
      builder.Entity<Participant>()
        .HasKey(p1 => p1.ParticipantId);

      builder.Entity<Participant>()
        .HasOne(p1 => p1.Hunt)
        .WithMany(p => p.Participants)
        .HasForeignKey(p1 => p1.HuntId)
        .OnDelete(DeleteBehavior.NoAction);

      builder.Entity<Participant>()
        .HasOne(p1 => p1.User)
        .WithMany(p => p.Participants)
        .HasForeignKey(p1 => p1.UserId)
        .OnDelete(DeleteBehavior.NoAction);

      //Unlock Action
      builder.Entity<UnlockAction>()
        .HasKey(u1 => u1.UnlockActionId);

      builder.Entity<UnlockAction>()
        .HasOne(u1 => u1.Lock)
        .WithMany(l => l.UnlockActions)
        .HasForeignKey(u1 => u1.LockId)
        .OnDelete(DeleteBehavior.NoAction);

      builder.Entity<UnlockAction>()
        .HasOne(u1 => u1.HuntObject)
        .WithMany(h => h.UnlockActions)
        .HasForeignKey(u1 => u1.HuntObjectId)
        .OnDelete(DeleteBehavior.NoAction);

      //Question
      builder.Entity<Question>()
        .HasKey(q1 => q1.QuestionId);

      builder.Entity<Question>()
        .HasOne(q1 => q1.Lock)
        .WithMany(l => l.Questions)
        .HasForeignKey(q1 => q1.LockId);

      var userId1 = Guid.NewGuid().ToString();
      var userId2 = Guid.NewGuid().ToString();
      builder.Entity<User>()
              .HasData(
                  new
                  {
                    UserId = userId1,
                    Username = "Snazzy101",
                    Password = "1234",
                    Email = "xxxx@example.com",
                    FirstName = "Robert",
                    LastName = "Roe",
                  }, new
                  {
                    UserId = userId2,
                    Username = "PsychoRedHead16",
                    Password = "1234",
                    Email = "xxxx@example.com",
                    FirstName = "Jakki",
                    LastName = "Crampton",
                  }
      );

      var huntId = Guid.NewGuid().ToString();
      var huntId2 = Guid.NewGuid().ToString();
      builder.Entity<Hunt>()
              .HasData(
                  new
                  {
                    HuntId = huntId,
                    UserId = userId1,
                    Title = "Hunt for the Red October"
                  }, new
                  {
                    HuntId = huntId2,
                    UserId = userId2,
                    Title = "Hunt for the Orange October"
                  }
      );

      var huntObjectId1 = Guid.NewGuid().ToString();
      var huntObjectId2 = Guid.NewGuid().ToString();
      builder.Entity<HuntObject>()
              .HasData(
                  new
                  {
                    HuntObjectId = huntObjectId1,
                    HuntId = huntId,
                    Order = 0,
                    Coordinates = "Blah",
                    Title = "Secret Location 1",
                    Text = "Here we go this is a secret place",
                    Type = 0,
                    Visible = true,
                    DefaultVisible = true
                  }, new
                  {
                    HuntObjectId = huntObjectId2,
                    HuntId = huntId,
                    Order = 1,
                    Coordinates = "Blah",
                    Title = "Secret Location 2",
                    Text = "Here we go this is a secret place",
                    Type = 0,
                    Visible = false,
                    DefaultVisible = false
                  }
      );

      var lockId = Guid.NewGuid().ToString();
      builder.Entity<Lock>()
              .HasData(
                  new
                  {
                    LockId = lockId,
                    HuntObjectId = huntObjectId1,
                    Type = 0,
                    Order = 0,
                    Locked = true
                  }
      );

      var questionId = Guid.NewGuid().ToString();
      builder.Entity<Question>()
              .HasData(
                  new
                  {
                    QuestionId = questionId,
                    LockId = lockId,
                    Type = 0,
                    Order = 0,
                    Answered = false,
                    Text = "What color is Red?",
                    Answer = "Red",
                    Hint = "The answer is Red"
                  }
      );

      var unlockActionId = Guid.NewGuid().ToString();
      builder.Entity<UnlockAction>()
              .HasData(
                  new
                  {
                    UnlockActionId = unlockActionId,
                    LockId = lockId,
                    HuntObjectId = huntObjectId2
                  }

      );

      var participantId = Guid.NewGuid().ToString();
      var participantId2 = Guid.NewGuid().ToString();
      builder.Entity<Participant>()
              .HasData(
                  new
                  {
                    ParticipantId = participantId,
                    HuntId = huntId,
                    UserId = userId2,
                    Accepted = false
                  }, new
                  {
                    ParticipantId = participantId2,
                    HuntId = huntId2,
                    UserId = userId1,
                    Accepted = false
                  }
      );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder.UseSqlServer(Helper.GetSetting("SQL-SERVER_DATABASE_CONNECTION_STRING"));
      }
    }
  }
}
