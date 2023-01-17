using System.Threading.Tasks;
using TreasureHunt.Models;
using System.Collections.Generic;
using System.Security;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace TreasureHunt.Services;

/* 
Converts Models to and from ViewModels.
ViewModels are sent to client to avoid recursive object
references in the Entity Framework. 
*/
public interface IViewModelService
{
  Task<List<Hunt_ViewModel>> To_Hunt_ViewModels(List<Hunt> hunts);
  Task<Hunt_ViewModel> To_Hunt_ViewModel(Hunt hunt);
  Task<List<Participant_ViewModel>> To_Participant_ViewModels(List<Participant> participants);
  List<UnlockAction_ViewModel> To_UnlockAction_ViewModels(List<UnlockAction> unlockActions);
  List<Question_ViewModel> To_Question_ViewModels(List<Question> questions);
  List<HuntObject_ViewModel> To_HuntObject_ViewModels(List<HuntObject> huntObjects);
  HuntObject_ViewModel To_HuntObject_ViewModel(HuntObject huntObject);
  Task<Participant_ViewModel> To_Participant_ViewModel(Participant participant);
  Lock_ViewModel To_Lock_ViewModel(Lock theLock);
  Question_ViewModel To_Question_ViewModel(Question question);
  UnlockAction_ViewModel To_UnlockAction_ViewModel(UnlockAction unlockAction);
}

public class ViewModelService : IViewModelService
{

  private readonly IDatabaseService _databaseService;

  public ViewModelService(IDatabaseService databaseService)
  {
    _databaseService = databaseService;
  }

  public async Task<List<Hunt_ViewModel>> To_Hunt_ViewModels(List<Hunt> hunts)
  {
    // List<Hunt> hunts = await _databaseService.GetUsersHunts(auth.Username);
    List<Hunt_ViewModel> viewModels = new List<Hunt_ViewModel>();
    foreach (var hunt in hunts)
    {
      List<Participant> participants = await _databaseService.GetHuntsParticipants(hunt.HuntId);
      viewModels.Add(
        new Hunt_ViewModel
        {
          Participants = await To_Participant_ViewModels(participants),
          HuntId = hunt.HuntId,
          UserId = hunt.UserId,
          Username = hunt.User.Username,
          Title = hunt.Title,
          Image = FileSystemService.GetImagePathFromLocalFileSystem(hunt.UserId, hunt.HuntId)
        }
      );
    }
    return viewModels;
  }

  public async Task<Hunt_ViewModel> To_Hunt_ViewModel(Hunt hunt)
  {
    // List<Hunt> hunts = await _databaseService.GetUsersHunts(auth.Username);
    List<Participant> participants = await _databaseService.GetHuntsParticipants(hunt.HuntId);
    Hunt_ViewModel viewModel = new Hunt_ViewModel
    {
      Participants = await To_Participant_ViewModels(participants),
      HuntId = hunt.HuntId,
      UserId = hunt.UserId,
      Username = hunt.User.Username,
      Title = hunt.Title,
      Image = FileSystemService.GetImagePathFromLocalFileSystem(hunt.UserId, hunt.HuntId)
    };
    return viewModel;
  }

  public async Task<List<Participant_ViewModel>> To_Participant_ViewModels(List<Participant> participants)
  {
    List<Participant_ViewModel> participantViewModels = new List<Participant_ViewModel>();
    foreach (var participant in participants)
    {
      User invitedUser = await _databaseService.GetUserById(participant.UserId);
      User owningUser = await _databaseService.GetUserById(participant.Hunt.UserId);
      participantViewModels.Add(
        new Participant_ViewModel
        {
          ParticipantId = participant.ParticipantId,
          HuntId = participant.HuntId,
          HuntTitle = participant.Hunt.Title,
          HuntOwner = owningUser.Username,
          UserId = participant.UserId,
          Username = invitedUser.Username,
          Accepted = participant.Accepted
        }
      );
    }
    return participantViewModels;
  }

  public List<UnlockAction_ViewModel> To_UnlockAction_ViewModels(List<UnlockAction> unlockActions)
  {
    List<UnlockAction_ViewModel> unlockActionViewModels = new List<UnlockAction_ViewModel>();
    foreach (var unlockAction in unlockActions)
    {
      unlockActionViewModels.Add(
        new UnlockAction_ViewModel
        {
          UnlockActionId = unlockAction.UnlockActionId,
          LockId = unlockAction.LockId,
          HuntObjectId = unlockAction.HuntObjectId
        }
      );
    }
    return unlockActionViewModels;
  }

  public List<Question_ViewModel> To_Question_ViewModels(List<Question> questions)
  {
    List<Question_ViewModel> questionViewModels = new List<Question_ViewModel>();
    foreach (var question in questions)
    {
      questionViewModels.Add(
        new Question_ViewModel
        {
          QuestionId = question.QuestionId,
          LockId = question.LockId,
          Type = question.Type,
          Order = question.Order,
          Answered = question.Answered,
          Text = question.Text,
          Answer = question.Answer,
          Hint = question.Hint
        }
      );
    }
    return questionViewModels;
  }

  public Question_ViewModel To_Question_ViewModel(Question question)
  {
    Question_ViewModel questionViewModel = new Question_ViewModel
    {
      QuestionId = question.QuestionId,
      LockId = question.LockId,
      Type = question.Type,
      Order = question.Order,
      Answered = question.Answered,
      Text = question.Text,
      Answer = question.Answer,
      Hint = question.Hint
    };
    return questionViewModel;
  }

  public List<Lock_ViewModel> To_Lock_ViewModels(List<Lock> locks)
  {
    List<Lock_ViewModel> lockViewModels = new List<Lock_ViewModel>();
    foreach (var eachlock in locks)
    {
      lockViewModels.Add(
        new Lock_ViewModel
        {
          LockId = eachlock.LockId,
          HuntObjectId = eachlock.HuntObjectId,
          Type = eachlock.Type,
          Order = eachlock.Order,
          Locked = eachlock.Locked,
          UnlockActions = To_UnlockAction_ViewModels(eachlock.UnlockActions.ToList()),
          Questions = To_Question_ViewModels(eachlock.Questions.ToList())
        }
      );
    }
    return lockViewModels;
  }

  public Lock_ViewModel To_Lock_ViewModel(Lock theLock)
  {
    Lock_ViewModel lockViewModels = new Lock_ViewModel
    {
      LockId = theLock.LockId,
      HuntObjectId = theLock.HuntObjectId,
      Type = theLock.Type,
      Order = theLock.Order,
      Locked = theLock.Locked,
      UnlockActions = To_UnlockAction_ViewModels(theLock.UnlockActions.ToList()),
      Questions = To_Question_ViewModels(theLock.Questions.ToList())
    };
    return lockViewModels;
  }

  public List<HuntObject_ViewModel> To_HuntObject_ViewModels(List<HuntObject> huntObjects)
  {
    List<HuntObject_ViewModel> huntObjectViewModels = new List<HuntObject_ViewModel>();
    foreach (var huntObject in huntObjects)
    {
      ICollection<Lock> locks = huntObject.Locks;
      huntObjectViewModels.Add(
        new HuntObject_ViewModel
        {
          HuntObjectId = huntObject.HuntObjectId,
          HuntId = huntObject.HuntId,
          Order = huntObject.Order,
          Coordinates = huntObject.Coordinates,
          Title = huntObject.Title,
          Text = huntObject.Text,
          Type = huntObject.Type,
          Visible = huntObject.Visible,
          DefaultVisible = huntObject.DefaultVisible,
          Locks = To_Lock_ViewModels(huntObject.Locks.ToList()),
          Image = FileSystemService.GetImagePathFromLocalFileSystem(huntObject.Hunt.UserId, huntObject.HuntObjectId)
        }
      );
    }
    return huntObjectViewModels;
  }

  public HuntObject_ViewModel To_HuntObject_ViewModel(HuntObject huntObject)
  {
    ICollection<Lock> locks = huntObject.Locks;
    HuntObject_ViewModel huntObjectViewModel = new HuntObject_ViewModel
    {
      HuntObjectId = huntObject.HuntObjectId,
      HuntId = huntObject.HuntId,
      Order = huntObject.Order,
      Coordinates = huntObject.Coordinates,
      Title = huntObject.Title,
      Text = huntObject.Text,
      Type = huntObject.Type,
      Visible = huntObject.Visible,
      DefaultVisible = huntObject.DefaultVisible,
      Locks = To_Lock_ViewModels(huntObject.Locks.ToList()),
      Image = FileSystemService.GetImagePathFromLocalFileSystem(huntObject.Hunt.UserId, huntObject.HuntObjectId)
    };
    return huntObjectViewModel;
  }

  public async Task<Participant_ViewModel> To_Participant_ViewModel(Participant participant)
  {
    User invitedUser = await _databaseService.GetUserById(participant.UserId);
    User owningUser = await _databaseService.GetUserById(participant.Hunt.UserId);
    Participant_ViewModel participantViewModel = new Participant_ViewModel
    {
      ParticipantId = participant.ParticipantId,
      HuntId = participant.HuntId,
      HuntTitle = participant.Hunt.Title,
      HuntOwner = owningUser.Username,
      UserId = participant.UserId,
      Username = invitedUser.Username,
      Accepted = participant.Accepted
    };
    return participantViewModel;
  }

  public UnlockAction_ViewModel To_UnlockAction_ViewModel(UnlockAction unlockAction)
  {
    UnlockAction_ViewModel unlockActionViewModel = new UnlockAction_ViewModel
    {
      UnlockActionId = unlockAction.UnlockActionId,
      LockId = unlockAction.LockId,
      HuntObjectId = unlockAction.HuntObjectId
    };
    return unlockActionViewModel;
  }
}