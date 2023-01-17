using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TreasureHunt.Data;
using System;
using System.Linq;
using TreasureHunt.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Web.Http;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Http;

namespace TreasureHunt.Services;

/*
Interacts with the database to perform CRUD operations.
*/
public interface IDatabaseService
{
  Task<IActionResult> GetAllDataForTesting();
  Task<User> Login(string username, string password);
  Task<IActionResult> GetUsersHunts(string username);
  Task<List<Participant>> GetHuntsParticipants(string huntId);
  Task<User> GetUserById(string userId);
  Task<Hunt> GetHuntById(string huntId);
  Task<IActionResult> CreateHunt(string userId, string title, HttpRequest req);
  Task<IActionResult> DeleteHunt(string huntId, string tokenUserId);
  Task<IActionResult> EditHunt(string tokenUserId, string title, string huntId, HttpRequest req);
  Task<IActionResult> GetHuntsObjects(string huntId, string tokenUserId);
  Task<IActionResult> CreateHuntObject(string huntId, int order, string coordinates, string title, string text, int type, bool defaultVisible, string tokenUserId, HttpRequest req);
  Task<IActionResult> EditHuntObject(int order, string coordinates, string title, string text, int type, bool defaultVisible, string tokenUserId, string huntObjectId, HttpRequest req);
  Task<IActionResult> DeleteHuntObject(string huntObjectId, string tokenUserId);
  Task<IActionResult> CreateParticipant(string huntId, string invitedUserId, string tokenUserId);
  Task<IActionResult> AcceptParticipant(string participantId, string tokenUserId);
  Task<IActionResult> GetUsersParticipants(string tokenUserId);
  Task<IActionResult> CreateLock(string huntObjectId, int type, int order, string tokenUserId);
  Task<IActionResult> EditLock(string lockId, int type, int order, string tokenUserId);
  Task<IActionResult> DeleteLock(string lockId, string tokenUserId);
  Task<IActionResult> CreateQuestion(string lockId, int type, int order, string text, string answer, string hint, string tokenUserId);
  Task<IActionResult> EditQuestion(string questionId, int type, int order, string text, string answer, string hint, string tokenUserId);
  Task<IActionResult> DeleteQuestion(string questionId, string tokenUserId);
  Task<IActionResult> CreateUnlockAction(string lockId, string huntObjectId, string tokenUserId);
  Task<IActionResult> EditUnlockAction(string unlockActionId, string huntObjectId, string tokenUserId);
  Task<IActionResult> DeleteUnlockAction(string unlockActionId, string tokenUserId);
  Task<IActionResult> DeleteParticipant(string participantId, string tokenUserId);
}

/*
Interacts with the database to perform CRUD operations.
*/
public class DatabaseService : IDatabaseService
{
  /* Instance of the db context created with dependency injection. */
  private readonly TreasureHuntContext _context;

  public DatabaseService(TreasureHuntContext context)
  {
    _context = context;
  }


  public async Task<User> Login(string username, string password)
  {
    var userResult = _context.Users.FirstOrDefault(u => u.Username == username);
    if (userResult == null)
    {
      return null;
    }

    //TODO: add salted hashing for security
    if (userResult.Password.Equals(password))
    {
      return userResult;
    }
    else
    {
      return null;
    }
  }


  public async Task<User> GetUserById(string userId)
  {
    if (!GenericValidations.IsValidGUID(userId))
    {
      return null; //Possibly throw error?
    }
    var userResult = _context.Users.Include(u => u.Participants).ThenInclude(p => p.Hunt).FirstOrDefault(u => u.UserId == userId);
    if (userResult == null)
    {
      return null;
    }
    else
    {
      return userResult;
    }
  }


  public async Task<IActionResult> GetUsersHunts(string username)
  {
    if (!GenericValidations.IsValidCharacters(username))
    {
      return new BadRequestResult();
    }

    var userResult = _context.Users.Include(u => u.Hunts).FirstOrDefault(u => u.Username == username);
    if (userResult == null)
    {
      return new BadRequestResult();
    }
    return new OkObjectResult(userResult.Hunts);
  }


  public async Task<IActionResult> CreateHunt(string userId, string title, HttpRequest req)
  {
    //Validate the userId
    if (!GenericValidations.IsValidGUID(userId))
    {
      return new BadRequestResult();
    }

    //Validate the user exists
    var userResult = _context.Users.FirstOrDefault(u => u.UserId == userId);
    if (userResult == null)
    {
      return new BadRequestResult();
    }


    //TODO: Escape dangerous characters in the title


    Hunt newHunt = new Hunt
    {
      HuntId = Guid.NewGuid().ToString(),
      UserId = userId,
      Title = title
    };


    // If an image is provided, save it.
    dynamic files = req.Form.Files;
    if (files != null)
    {
      string imageSaveResult = await FileSystemService.SaveImageToLocalFileSystem(req, userId, newHunt.HuntId);
      if (imageSaveResult == null)
      {
        return new BadRequestResult();
      }
    }

    _context.Hunts.Add(newHunt);
    await _context.SaveChangesAsync();
    return new OkObjectResult(newHunt);
  }


  public async Task<IActionResult> DeleteHunt(string huntId, string tokenUserId)
  {
    if (!GenericValidations.IsValidGUID(huntId))
    {
      return new BadRequestResult();
    }

    Hunt huntToDelete = await GetHuntById(huntId);

    if (huntToDelete == null)
    {
      return new BadRequestResult();
    }

    if (!huntToDelete.UserId.Equals(tokenUserId))
    {
      return new UnauthorizedResult();
    }

    string imageDeleteResult = FileSystemService.DeleteImageFromLocalFileSystem(tokenUserId, huntId);
    if (imageDeleteResult == null)
    {
      return new BadRequestResult();
    }

    foreach (var participant in huntToDelete.Participants)
    {
      _context.Participants.Remove(participant);
    }

    foreach (var huntObject in huntToDelete.HuntObjects)
    {
      foreach (var eachLock in huntObject.Locks)
      {
        foreach (var unlockAction in eachLock.UnlockActions)
        {
          _context.UnlockActions.Remove(unlockAction);
        }
      }
    }
    _context.Hunts.Remove(huntToDelete);
    await _context.SaveChangesAsync();
    return new OkObjectResult("Deleted");
  }


  public async Task<IActionResult> EditHunt(string tokenUserId, string title, string huntId, HttpRequest req)
  {
    //Check that the userId is valid,
    //The user exists,
    //The huntId is valid,
    //The hunt exists,
    //The hunt is owned by the token holder
    if (!GenericValidations.IsValidGUID(tokenUserId))
    {
      return new BadRequestResult();
    }
    var userResult = _context.Users.FirstOrDefault(u => u.UserId == tokenUserId);
    if (userResult == null)
    {
      return new BadRequestResult();
    }
    if (!GenericValidations.IsValidGUID(huntId))
    {
      return new BadRequestResult();
    }
    Hunt huntToUpdate = await GetHuntById(huntId);
    if (huntToUpdate == null)
    {
      return new BadRequestResult();
    }
    if (!huntToUpdate.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }
    //TODO: Escape dangerous characters in the title
    huntToUpdate.Title = title;

    // If an image is provided, save it and replace any old one.
    dynamic files = req.Form.Files;
    if (files != null)
    {
      string imageDeleteResult = FileSystemService.DeleteImageFromLocalFileSystem(tokenUserId, huntId);
      string imageSaveResult = await FileSystemService.SaveImageToLocalFileSystem(req, tokenUserId, huntId);
      if (imageSaveResult == null)
      {
        return new BadRequestResult();
      }
    }

    _context.Hunts.Update(huntToUpdate);
    await _context.SaveChangesAsync();
    return new OkObjectResult(huntToUpdate);
  }


  public async Task<Hunt> GetHuntById(string huntId)
  {
    if (!GenericValidations.IsValidGUID(huntId))
    {
      return null;
    }
    var huntResult = _context.Hunts.Include(h => h.Participants).Include(h => h.HuntObjects).ThenInclude(o => o.Locks).ThenInclude(l => l.UnlockActions).FirstOrDefault(h => h.HuntId == huntId);
    if (huntResult == null)
    {
      return null;
    }
    else
    {
      return huntResult;
    }
  }


  public async Task<IActionResult> GetHuntsObjects(string huntId, string tokenUserId)
  {
    if (!GenericValidations.IsValidGUID(huntId))
    {
      return new BadRequestResult();
    }

    Hunt hunt = await GetHuntById(huntId);
    if (hunt == null)
    {
      return new BadRequestResult();
    }
    if (!hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }
    List<HuntObject> objects = _context.HuntObjects
                                  .Where(o => o.HuntId == huntId)
                                  .Include(o => o.Locks)
                                  .ThenInclude(l => l.UnlockActions)
                                  .Include(o => o.Locks)
                                  .ThenInclude(l => l.Questions)
                                  .ToList();

    if (objects == null)
    {
      objects = new List<HuntObject>();
    }

    return new OkObjectResult(objects);
  }


  public async Task<IActionResult> CreateHuntObject(string huntId, int order, string coordinates, string title, string text, int type, bool defaultVisible, string tokenUserId, HttpRequest req)
  {
    //Validate the UserId and HuntId
    if (!GenericValidations.IsValidGUID(tokenUserId) || !GenericValidations.IsValidGUID(huntId))
    {
      return new BadRequestResult();
    }

    //Validate the user exists
    var userResult = _context.Users.FirstOrDefault(u => u.UserId == tokenUserId);
    if (userResult == null)
    {
      return new BadRequestResult();
    }

    //Validate the hunt exists and the hunt is owned by the token holder
    var huntResult = _context.Hunts.FirstOrDefault(h => h.HuntId == huntId);
    if (huntResult == null)
    {
      return new BadRequestResult();
    }
    else if (!huntResult.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    HuntObject newHuntObject = new HuntObject
    {
      HuntObjectId = Guid.NewGuid().ToString(),
      HuntId = huntId,
      Order = order,
      Coordinates = coordinates,
      Title = title,
      Text = text,
      Type = type,
      Visible = defaultVisible,
      DefaultVisible = defaultVisible
    };

    // If an image is provided, save it.
    dynamic files = req.Form.Files;
    if (files != null)
    {
      string imageSaveResult = await FileSystemService.SaveImageToLocalFileSystem(req, tokenUserId, newHuntObject.HuntObjectId);
      if (imageSaveResult == null)
      {
        return new BadRequestResult();
      }
    }

    _context.HuntObjects.Add(newHuntObject);
    await _context.SaveChangesAsync();
    return new OkObjectResult(newHuntObject);
  }


  public async Task<IActionResult> EditHuntObject(int order, string coordinates, string title, string text, int type, bool defaultVisible, string tokenUserId, string huntObjectId, HttpRequest req)
  {
    //Validate the UserId, HuntId, and HuntObjectId
    if (!GenericValidations.IsValidGUID(tokenUserId)
     || !GenericValidations.IsValidGUID(huntObjectId))
    {
      return new BadRequestResult();
    }

    //Validate the user exists
    var userResult = _context.Users.FirstOrDefault(u => u.UserId == tokenUserId);
    if (userResult == null)
    {
      return new BadRequestResult();
    }

    HuntObject huntObjectToUpdate = await GetHuntObjectById(huntObjectId);
    if (huntObjectToUpdate == null)
    {
      return new BadRequestResult();
    }

    //Validate the huntObject is owned by the token holder
    if (!huntObjectToUpdate.Hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    huntObjectToUpdate.Order = order;
    huntObjectToUpdate.Coordinates = coordinates;
    huntObjectToUpdate.Title = title;
    huntObjectToUpdate.Text = text;
    huntObjectToUpdate.Type = type;
    huntObjectToUpdate.DefaultVisible = defaultVisible;

    // If an image is provided, save it and replace any old one.
    dynamic files = req.Form.Files;
    if (files != null)
    {
      string imageDeleteResult = FileSystemService.DeleteImageFromLocalFileSystem(tokenUserId, huntObjectId);
      string imageSaveResult = await FileSystemService.SaveImageToLocalFileSystem(req, tokenUserId, huntObjectId);
      if (imageSaveResult == null)
      {
        return new BadRequestResult();
      }
    }

    _context.HuntObjects.Update(huntObjectToUpdate);
    await _context.SaveChangesAsync();
    return new OkObjectResult(huntObjectToUpdate);
  }


  public async Task<HuntObject> GetHuntObjectById(string huntObjectId)
  {
    if (!GenericValidations.IsValidGUID(huntObjectId))
    {
      return null;
    }
    var huntObjectResult = _context.HuntObjects.Include(o => o.Hunt).Include(o => o.Locks).ThenInclude(l => l.UnlockActions).Include(o => o.UnlockActions).FirstOrDefault(o => o.HuntObjectId == huntObjectId);
    if (huntObjectResult == null)
    {
      return null;
    }
    else
    {
      return huntObjectResult;
    }
  }


  public async Task<IActionResult> DeleteHuntObject(string huntObjectId, string tokenUserId)
  {
    if (!GenericValidations.IsValidGUID(huntObjectId))
    {
      return new BadRequestResult();
    }

    HuntObject huntObjectToDelete = await GetHuntObjectById(huntObjectId);

    if (huntObjectToDelete == null)
    {
      return new BadRequestResult();
    }

    if (!huntObjectToDelete.Hunt.UserId.Equals(tokenUserId))
    {
      return new UnauthorizedResult();
    }

    string imageDeleteResult = FileSystemService.DeleteImageFromLocalFileSystem(tokenUserId, huntObjectId);
    if (imageDeleteResult == null)
    {
      return new BadRequestResult();
    }

    foreach (var unlockAction in huntObjectToDelete.UnlockActions)
    {
      _context.UnlockActions.Remove(unlockAction);
    }

    foreach (var eachLock in huntObjectToDelete.Locks)
    {
      foreach (var unlockAction in eachLock.UnlockActions)
      {
        _context.UnlockActions.Remove(unlockAction);
      }
    }

    _context.HuntObjects.Remove(huntObjectToDelete);
    await _context.SaveChangesAsync();
    return new OkObjectResult("Deleted");
  }

  public async Task<IActionResult> CreateParticipant(string huntId, string invitedUserId, string tokenUserId)
  {
    //Validate the UserId and HuntId
    if (!GenericValidations.IsValidGUID(tokenUserId)
     || !GenericValidations.IsValidGUID(huntId)
     || !GenericValidations.IsValidGUID(invitedUserId))
    {
      return new BadRequestResult();
    }

    //Validate the users exist
    var invitedUserResult = _context.Users.FirstOrDefault(u => u.UserId == invitedUserId);
    var tokenUserResult = _context.Users.FirstOrDefault(u => u.UserId == tokenUserId);
    var hunt = await GetHuntById(huntId);
    var existingParticipant = _context.Participants.FirstOrDefault(p => p.UserId == invitedUserId && p.HuntId == huntId);
    if (invitedUserResult == null
     || tokenUserResult == null
     || hunt == null
     || invitedUserId.Equals(tokenUserId))
    {
      return new BadRequestResult();
    }

    if (existingParticipant != null)
    {
      return new OkObjectResult(existingParticipant);
    }

    //Validate the hunt is owned by the token holder
    if (!hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    //Create the participant
    Participant newParticipant = new Participant
    {
      ParticipantId = Guid.NewGuid().ToString(),
      HuntId = huntId,
      UserId = invitedUserId,
      Accepted = false
    };

    _context.Participants.Add(newParticipant);
    await _context.SaveChangesAsync();
    return new OkObjectResult(newParticipant);
  }

  public async Task<IActionResult> AcceptParticipant(string participantId, string tokenUserId)
  {
    //Validate the UserId and HuntId
    if (!GenericValidations.IsValidGUID(tokenUserId)
     || !GenericValidations.IsValidGUID(participantId))
    {
      return new BadRequestResult();
    }

    //Validate the user and participant exist
    var tokenUserResult = _context.Users.FirstOrDefault(u => u.UserId == tokenUserId);
    var participantResult = _context.Participants.FirstOrDefault(p => p.ParticipantId == participantId);

    if (tokenUserResult == null || participantResult == null)
    {
      return new BadRequestResult();
    }

    //Validate the hunt is owned by the token holder
    if (!participantResult.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    participantResult.Accepted = true;

    _context.Participants.Update(participantResult);
    await _context.SaveChangesAsync();

    return new OkObjectResult(participantResult);
  }

  public async Task<IActionResult> DeleteParticipant(string participantId, string tokenUserId)
  {
    // Allowed to if you own the hunt or are the invited user
    if (!GenericValidations.IsValidGUID(participantId)
     || !GenericValidations.IsValidGUID(tokenUserId))
    {
      return new BadRequestResult();
    }

    Participant participantToDelete = await GetParticipantById(participantId);

    // If the participent exists and either the token bearer is the invited user or the owner of the hunt
    if (participantToDelete == null
    || (!participantToDelete.UserId.Equals(tokenUserId) && !participantToDelete.Hunt.UserId.Equals(tokenUserId)))
    {
      return new ForbidResult();
    }

    _context.Participants.Remove(participantToDelete);
    await _context.SaveChangesAsync();
    return new OkObjectResult("Deleted");
  }

  public async Task<Participant> GetParticipantById(string participantId)
  {
    if (!GenericValidations.IsValidGUID(participantId))
    {
      return null;
    }
    var participantResult = _context.Participants.Include(p => p.Hunt).FirstOrDefault(p => p.ParticipantId == participantId);
    if (participantResult == null)
    {
      return null;
    }
    else
    {
      return participantResult;
    }
  }

  public async Task<List<Participant>> GetHuntsParticipants(string huntId)
  {
    if (!GenericValidations.IsValidGUID(huntId))
    {
      return null; //Possibly throw error?
    }
    return (from p in _context.Participants
            where p.HuntId == huntId
            select p).ToList();
  }

  public async Task<IActionResult> GetUsersParticipants(string tokenUserId)
  {
    //Validate the UserId
    if (!GenericValidations.IsValidGUID(tokenUserId))
    {
      return new BadRequestResult();
    }

    User user = await GetUserById(tokenUserId);
    if (user == null)
    {
      return new BadRequestResult();
    }
    else
    {
      List<Participant> participants = user.Participants.ToList();
      return new OkObjectResult(participants);
    }
  }

  public async Task<IActionResult> CreateLock(string huntObjectId, int type, int order, string tokenUserId)
  {
    // validate parameters
    if (!GenericValidations.IsValidGUID(huntObjectId) || !GenericValidations.IsValidGUID(tokenUserId))
    {
      return new BadRequestResult();
    }

    HuntObject locksHuntObject = await GetHuntObjectById(huntObjectId);
    if (locksHuntObject == null || !locksHuntObject.Hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    //Create lock
    Lock newLock = new Lock
    {
      LockId = Guid.NewGuid().ToString(),
      HuntObjectId = huntObjectId,
      Type = type,
      Order = order,
      Locked = true
    };

    //Persist to database and return 
    _context.Locks.Add(newLock);
    await _context.SaveChangesAsync();
    return new OkObjectResult(newLock);
  }

  public async Task<IActionResult> EditLock(string lockId, int type, int order, string tokenUserId)
  {
    // validate the huntObjectId, tokenUserId, and lockId
    if (!GenericValidations.IsValidGUID(tokenUserId)
     || !GenericValidations.IsValidGUID(lockId))
    {
      return new BadRequestResult();
    }

    // validate the huntObject and lock exist.
    // and that the huntObject and lock belongs to the token bearer.
    Lock lockToEdit = await GetLockById(lockId);
    if (lockToEdit == null)
    {
      return new BadRequestResult();
    }

    HuntObject locksHuntObject = await GetHuntObjectById(lockToEdit.HuntObjectId);

    if (locksHuntObject == null
    || !locksHuntObject.Hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    lockToEdit.Type = type;
    lockToEdit.Order = order;

    _context.Locks.Update(lockToEdit);
    await _context.SaveChangesAsync();
    return new OkObjectResult(lockToEdit);
  }

  public async Task<Lock> GetLockById(string lockId)
  {
    if (!GenericValidations.IsValidGUID(lockId))
    {
      return null; //Possibly throw error?
    }
    var lockResult = _context.Locks.Include(l => l.HuntObject).ThenInclude(o => o.Hunt).Include(l => l.UnlockActions).FirstOrDefault(l => l.LockId == lockId);
    if (lockResult == null)
    {
      return null;
    }
    else
    {
      return lockResult;
    }

  }

  public async Task<IActionResult> DeleteLock(string lockId, string tokenUserId)
  {
    // validate the huntObjectId, tokenUserId, and lockId
    if (!GenericValidations.IsValidGUID(tokenUserId)
     || !GenericValidations.IsValidGUID(lockId))
    {
      return new BadRequestResult();
    }

    // validate the huntObject and lock exist.
    // and that the huntObject and lock belongs to the token bearer.
    Lock lockToRemove = await GetLockById(lockId);
    if (lockToRemove == null)
    {
      return new BadRequestResult();
    }

    HuntObject locksHuntObject = await GetHuntObjectById(lockToRemove.HuntObjectId);

    if (locksHuntObject == null
    || !locksHuntObject.Hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    foreach (var unlockAction in lockToRemove.UnlockActions)
    {
      _context.UnlockActions.Remove(unlockAction);
    }

    _context.Locks.Remove(lockToRemove);
    await _context.SaveChangesAsync();
    return new OkObjectResult("Deleted");
  }

  public async Task<IActionResult> CreateQuestion(string lockId, int type, int order, string text, string answer, string hint, string tokenUserId)
  {
    //Validate the lockId and the text parameters
    if (!GenericValidations.IsValidGUID(lockId)
     || !GenericValidations.IsValidCharacters(text)
     || !GenericValidations.IsValidCharacters(answer)
     || !GenericValidations.IsValidCharacters(hint))
    {
      return new BadRequestResult();
    }

    //Validate that the lock exists and is owned by the token bearer
    Lock theLock = await GetLockById(lockId);
    if (theLock == null
    || !theLock.HuntObject.Hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    //Create new Question
    Question newQuestion = new Question
    {
      QuestionId = Guid.NewGuid().ToString(),
      LockId = lockId,
      Type = type,
      Order = order,
      Answered = false,
      Text = text,
      Answer = answer,
      Hint = hint
    };

    //Persist the new Question and return it
    _context.Questions.Add(newQuestion);
    await _context.SaveChangesAsync();
    return new OkObjectResult(newQuestion);
  }

  public async Task<IActionResult> EditQuestion(string questionId, int type, int order, string text, string answer, string hint, string tokenUserId)
  {
    //Validate the questionId and the text parameters
    if (!GenericValidations.IsValidGUID(questionId)
     || !GenericValidations.IsValidGUID(tokenUserId)
     || !GenericValidations.IsValidCharacters(text)
     || !GenericValidations.IsValidCharacters(answer)
     || !GenericValidations.IsValidCharacters(hint))
    {
      return new BadRequestResult();
    }

    // Validate the Question exists and that the Question belongs to the token bearer
    Question questionToEdit = await GetQuestionById(questionId);
    if (questionToEdit == null || !questionToEdit.Lock.HuntObject.Hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    // Update the Question
    questionToEdit.Type = type;
    questionToEdit.Order = order;
    questionToEdit.Text = text;
    questionToEdit.Answer = answer;
    questionToEdit.Hint = hint;

    // Persist the changes to the Question
    _context.Questions.Update(questionToEdit);
    await _context.SaveChangesAsync();
    return new OkObjectResult(questionToEdit);
  }

  public async Task<IActionResult> DeleteQuestion(string questionId, string tokenUserId)
  {
    // Validate the Ids
    if (!GenericValidations.IsValidGUID(questionId)
     || !GenericValidations.IsValidGUID(tokenUserId))
    {
      return new BadRequestResult();
    }

    // Validate that the question exists and is owned by the token bearer
    Question questionToDelete = await GetQuestionById(questionId);
    if (questionToDelete == null || !questionToDelete.Lock.HuntObject.Hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    // Delete the question and save the context
    _context.Questions.Remove(questionToDelete);
    await _context.SaveChangesAsync();
    return new OkObjectResult("Deleted");
  }

  public async Task<Question> GetQuestionById(string questionId)
  {
    if (!GenericValidations.IsValidGUID(questionId))
    {
      return null; //Possibly throw error?
    }
    var questionResult = _context.Questions.Include(q => q.Lock).ThenInclude(l => l.HuntObject).ThenInclude(o => o.Hunt).FirstOrDefault(q => q.QuestionId == questionId);
    if (questionResult == null)
    {
      return null;
    }
    else
    {
      return questionResult;
    }
  }

  public async Task<IActionResult> CreateUnlockAction(string lockId, string huntObjectId, string tokenUserId)
  {
    // Validate the Ids
    if (!GenericValidations.IsValidGUID(lockId)
     || !GenericValidations.IsValidGUID(huntObjectId)
     || !GenericValidations.IsValidGUID(tokenUserId))
    {
      return new BadRequestResult();
    }

    // Validate that the lock and the huntObject exist and belong to the token bearer
    HuntObject huntObject = await GetHuntObjectById(huntObjectId);
    Lock theLock = await GetLockById(lockId);
    if (huntObject == null
     || theLock == null
     || !huntObject.Hunt.UserId.Equals(tokenUserId)
     || !theLock.HuntObject.Hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    // Also validate that the lock and huntObject are part of the same hunt
    // Also validate that the lock isnt a child of the huntObject
    if (!huntObject.HuntId.Equals(theLock.HuntObject.HuntId)
     || huntObject.HuntObjectId.Equals(theLock.HuntObjectId))
    {
      return new BadRequestResult();
    }

    // Create the UnlockAction and persist then return it.
    UnlockAction newUnlockAction = new UnlockAction
    {
      UnlockActionId = Guid.NewGuid().ToString(),
      LockId = lockId,
      HuntObjectId = huntObjectId
    };

    _context.UnlockActions.Add(newUnlockAction);
    await _context.SaveChangesAsync();
    return new OkObjectResult(newUnlockAction);
  }
  //EditUnlockAction
  public async Task<IActionResult> EditUnlockAction(string unlockActionId, string huntObjectId, string tokenUserId)
  {
    //Validate the ids
    if (!GenericValidations.IsValidGUID(unlockActionId)
     || !GenericValidations.IsValidGUID(huntObjectId)
     || !GenericValidations.IsValidGUID(tokenUserId))
    {
      return new BadRequestResult();
    }

    // Validate that the unlockAction and the huntObject exist and belong to the token bearer
    HuntObject huntObject = await GetHuntObjectById(huntObjectId);
    UnlockAction unlockAction = await GetUnlockActionById(unlockActionId);
    if (huntObject == null
     || unlockAction == null
     || !huntObject.Hunt.UserId.Equals(tokenUserId)
     || !unlockAction.Lock.HuntObject.Hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    // Also validate that the unlockAction and huntObject are part of the same hunt
    // Also validate that the unlockAction isnt a child of the huntObject
    if (!huntObject.HuntId.Equals(unlockAction.Lock.HuntObject.HuntId)
     || huntObject.HuntObjectId.Equals(unlockAction.Lock.HuntObjectId))
    {
      return new BadRequestResult();
    }

    // Update the UnlockAction
    unlockAction.HuntObjectId = huntObjectId;

    // Persist the changes to the UnlockAction then return it
    _context.UnlockActions.Update(unlockAction);
    await _context.SaveChangesAsync();
    return new OkObjectResult(unlockAction);
  }

  public async Task<IActionResult> DeleteUnlockAction(string unlockActionId, string tokenUserId)
  {
    // Validate the Ids
    if (!GenericValidations.IsValidGUID(unlockActionId)
     || !GenericValidations.IsValidGUID(tokenUserId))
    {
      return new BadRequestResult();
    }

    // Validate that the question exists and is owned by the token bearer
    UnlockAction unlockActionToDelete = await GetUnlockActionById(unlockActionId);
    if (unlockActionToDelete == null || !unlockActionToDelete.Lock.HuntObject.Hunt.UserId.Equals(tokenUserId))
    {
      return new ForbidResult();
    }

    // Delete the question and save the context
    _context.UnlockActions.Remove(unlockActionToDelete);
    await _context.SaveChangesAsync();
    return new OkObjectResult("Deleted");
  }

  public async Task<UnlockAction> GetUnlockActionById(string unlockActionId)
  {
    if (!GenericValidations.IsValidGUID(unlockActionId))
    {
      return null; //Possibly throw error?
    }
    var unlockActionResult = _context.UnlockActions.Include(u => u.Lock).ThenInclude(l => l.HuntObject).ThenInclude(o => o.Hunt).FirstOrDefault(u => u.UnlockActionId == unlockActionId);
    if (unlockActionResult == null)
    {
      return null;
    }
    else
    {
      return unlockActionResult;
    }
  }

  public async Task<IActionResult> GetAllDataForTesting()
  {
    // Pulls the whole database so I can see what happens when I CRUD everything.
    var Users = _context.Users.ToArray();
    var Hunts = _context.Hunts.ToArray();
    var HuntObjects = _context.HuntObjects.ToArray();
    var Locks = _context.Locks.ToArray();
    var Participants = _context.Participants.ToArray();
    var Questions = _context.Questions.ToArray();
    var UnlockActions = _context.UnlockActions.ToArray();


    string responseMessage = "***** Users *****\n";
    //Add the users
    for (int i = 0; i < Users.Length; ++i)
    {
      responseMessage += i + "--------\nName: " + Users[i].Username + "\nId: " + Users[i].UserId + "\n";
    }

    //Add the Hunts
    responseMessage += "\n***** Hunts *****\n";
    for (int i = 0; i < Hunts.Length; ++i)
    {
      responseMessage += i + "--------\nTitle: " + Hunts[i].Title + "\nOwner: " + Hunts[i].User.Username + "\n";
    }

    //Add the HuntObjects
    responseMessage += "\n***** HuntObjects *****\n";
    for (int i = 0; i < HuntObjects.Length; ++i)
    {
      responseMessage += i + "--------\nTitle: " + HuntObjects[i].Title + "\nHunt Title: " + HuntObjects[i].Hunt.Title + "\n";
    }

    //Add the Locks
    responseMessage += "\n***** Locks *****\n";
    for (int i = 0; i < Locks.Length; ++i)
    {
      responseMessage += i + "--------\nId: " + Locks[i].LockId + "\nHuntObject Title: " + Locks[i].HuntObject.Title + "\n";
    }

    //Add the Questions
    responseMessage += "\n***** Questions *****\n";
    for (int i = 0; i < Questions.Length; ++i)
    {
      responseMessage += i + "--------\nId: " + Questions[i].QuestionId + "\nLockId: " + Questions[i].LockId + "\n";
    }

    //Add the UnlockActions
    responseMessage += "\n***** UnlockActions *****\n";
    for (int i = 0; i < UnlockActions.Length; ++i)
    {
      responseMessage += i + "--------\nId: " + UnlockActions[i].UnlockActionId + "\nLockId: " + UnlockActions[i].LockId + "\nHuntObject Title to unlock: " + UnlockActions[i].HuntObject.Title + "\n";
    }

    //Add the Participants
    responseMessage += "\n***** Participants *****\n";
    for (int i = 0; i < Participants.Length; ++i)
    {
      responseMessage += i + "--------\nHunt Title: " + Participants[i].Hunt.Title + "\nUsername: " + Participants[i].User.Username + "\nAccepted: " + Participants[i].Accepted + "\n";
    }

    return new OkObjectResult(responseMessage);
  }
}