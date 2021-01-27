using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Common;
using SimpleForum.Common.Server;
using SimpleForum.Models;
using SimpleForum.Web.Models;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly SimpleForumRepository _repository;
        private int CommentsPerPage = 15;

        public UserController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        // Returns a user's profile
        public async Task<IActionResult> Index(int id, int page = 1)
        {
            // Retrieves users and return 404 if null
            User user = await _repository.GetUserAsync(id);
            if (user == null) return NotFound();
            
            // Retrieves comments and logged in user
            IEnumerable<UserComment> userComments = _repository.GetUserComments(user, page);
            User currentUser = await _repository.GetUserAsync(User);
            
            // Creates model and returns view
            UserPageViewModel model = new UserPageViewModel()
            {
                User = user,
                Page = page,
                PageCount = (user.UserPageComments.Count(x => !x.Deleted) + (CommentsPerPage - 1)) / CommentsPerPage,
                CurrentUser = currentUser,
                CurrentPageComments = userComments
            };
            return View("User", model);
        }

        // Posts a comment to a user's profile
        [Authorize]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(PreventMuted))]
        public async Task<IActionResult> PostUserComment(string content, int userPageID)
        {
            // Redirects if content is null
            if (content == null) return Redirect("/");
            
            // Creates and posts comment
            UserComment comment = new UserComment()
            {
                Content = content,
                DatePosted = DateTime.Now,
                UserID = Tools.GetUserID(User),
                UserPageID = userPageID,
            };
            Result result = await _repository.PostUserCommentAsync(comment);

            // Saves changes and redirects if successful, otherwise returns 403
            if (result.Success)
            {
                await _repository.SaveChangesAsync();
                return Redirect("/User?id=" + userPageID);
            }
            return StatusCode(result.Code);
        }

        // Deletes a posted UserComment
        [Authorize]
        [HttpPost]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> DeleteUserComment(int userCommentID)
        {
            // Deletes comment and returns error if access denied
            Result result = await _repository.DeleteUserCommentAsync(userCommentID);
            if (result.Failure) return Forbid();
            
            await _repository.SaveChangesAsync();
            
            
            // Returns the view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comment deleted",
                MessageTitle = "Comment deleted"
            };
            return View("Message", model);
        }

        // Returns the form for deleting a comment as an admin
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminDeleteUserComment(int userCommentID)
        {
            // Retrieves comment and returns view
            UserComment comment = await _repository.GetUserCommentAsync(userCommentID);
            return View(comment);
        }
        
        // Deletes a UserComment as an admin
        [Authorize]
        [HttpPost]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> AdminDeleteUserComment(int userCommentID, string reason)
        {
            // Deletes the comment and returns 404 if comment is already deleted
            Result result = await _repository.AdminDeleteUserCommentAsync(userCommentID, reason);
            if (result.Failure) return new StatusCodeResult(result.Code);
            await _repository.SaveChangesAsync();
            
            
            // Returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comment deleted",
                MessageTitle = "Comment deleted"
            };
            return View("Message", model);
        }

        // Returns a page for changing the comment settings of a user's profile
        [Authorize]
        public async Task<IActionResult> CommentSettings()
        {
            // Retrieves user and returns view
            User user = await _repository.GetUserAsync(User);
            return View(user);
        }

        // Locks a user's comments
        [Authorize]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> LockComments()
        {
            // Retrieves user and sets comments as locked
            User user = await _repository.GetUserAsync(User);
            user.CommentsLocked = true;
            await _repository.SaveChangesAsync();
            
            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comments locked",
                MessageContent = "Comments locked"
            };
            return View("Message", model);
        }

        // Clears a user's comments
        [Authorize]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> ClearComments()
        {
            // Retrieves user and deletes comments
            User user = await _repository.GetUserAsync(User);
            foreach (UserComment userComment in user.UserPageComments)
            {
                userComment.Deleted = true;
            }
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comments cleared",
                MessageTitle = "Comments cleared"
            };
            return View("Message", model);
        }

        // Unlocks a user's comments
        [Authorize]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> UnlockComments()
        {
            // Retrieves users and locks comments
            User user = await _repository.GetUserAsync(User);
            user.CommentsLocked = false;
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comments unlocked",
                MessageTitle = "Comments unlocked"
            };
            return View("Message", model);
        }

        // Returns a page of actions an admin can make on a user's profile
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminActions(int id)
        {
            // Retrieves user and returns view
            User user = await _repository.GetUserAsync(id);
            return View(user);
        }

        // Returns a page for muting a user
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MuteUser(int id)
        {
            // Retrieves user and returns view
            User user = await _repository.GetUserAsync(id);
            return View(user);
        }

        // Mutes a user
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendMuteUser(int id, string reason)
        {
            // Redirects if reason is empty
            if (reason == null) return Redirect("/");

            // Retrieves and mutes user
            User user = await _repository.GetUserAsync(id);
            user.Muted = true;
            user.MuteReason = reason;
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "User muted",
                MessageTitle = "User muted"
            };
            return View("Message", model);
        }

        // Unmutes a user
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendUnmuteUser(int id)
        {
            // Retrieves and unmutes user
            User user = await _repository.GetUserAsync(id);
            user.Muted = false;
            user.MuteReason = null;
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "User unmuted",
                MessageTitle = "User unmuted"
            };
            return View("Message", model);
        }
        
        // Returns a page for banning a user
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BanUser(int id)
        {
            // Retrieves user and returns view
            User user = await _repository.GetUserAsync(id);
            return View(user);
        }

        // Bans a user
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendBanUser(int id, string reason)
        {
            // Retrieves and bans user
            User user = await _repository.GetUserAsync(id);
            user.Banned = true;
            user.BanReason = reason;
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "User banned",
                MessageTitle = "User banned"
            };
            return View("Message", model);
        }

        // Unbans a user
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendUnbanUser(int id)
        {
            // Retrieves and unbans user
            User user = await _repository.GetUserAsync(id);
            user.Banned = false;
            user.BanReason = null;
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "User unbanned",
                MessageTitle = "User unbanned"
            };
            return View("Message", model);
        }

        // Returns a page for editing the user's profile information
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            // Retrieves user and returns view
            User user = await _repository.GetUserAsync(User);
            return View(new EditUserViewModel() { User = user });
        }
        
        // Updates a user's account information
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(string email, string password, string confirmPassword, string bio,
            IFormFile profilePicture)
        {
            // Returns if submitted passwords do not match
            if (password != confirmPassword) return RedirectToAction("Edit");
            
            // Retrieves user account and updates information
            User user = await _repository.GetUserAsync(User);
            Stream stream = profilePicture.OpenReadStream();
            await _repository.UpdateProfileAsync(email, password, bio, stream, user);
            
            // Redirects to profile
            return RedirectToAction("Index", new {id = user.UserID});
        }

        // Returns a list of the user's notifications
        [Authorize]
        public async Task<IActionResult> Notifications()
        {
            // Retrieves the user
            User user = await _repository.GetUserAsync(User);
            
            // Creates model and returns view
            NotificationsViewModel model = new NotificationsViewModel()
            {
                Notifications = user.Notifications.OrderByDescending(x => x.DateCreated)
            };
            return View(model);
        }

        // Displays a notification of the given id
        [Authorize]
        public async Task<IActionResult> Notification(int id)
        {
            // Retrieves notification from database and return 403 if not owner
            Notification notification = await _repository.GetNotificationAsync(id);
            if (Tools.GetUserID(User) == notification.UserID) return Forbid();
            
            // Sets notification as read if unread
            if (!notification.Read)
            {
                notification.Read = true;
                await _repository.SaveChangesAsync();
            }
            
            // Returns view with notification
            return View(notification);
        }

        // Initiates the deletion of a user's account
        [Authorize]
        [ServiceFilter(typeof(CheckPassword))]
        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            // Retrieves user and starts account deletion
            User user = await _repository.GetUserAsync(User);
            await _repository.StartDeleteAccountAsync(user);
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Delete account",
                MessageTitle = "Delete account",
                MessageContent = "A confirmation email has been sent to your email account. This email is valid for one hour."
            };
            return View("Message", model);
        }

        // Deletes the user's profile
        [Authorize]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendDelete(string code)
        {
            // Redirects to index if code is null
            if (code == null) return Redirect("/");
            
            // Retrieves user and email and verified the correct user is signed in.
            User user = await _repository.GetUserAsync(User);
            EmailCode emailCode = await _repository.GetEmailCodeAsync(code);
            if (user != emailCode.User) return Forbid();
            
            // Returns message if code is no longer valid
            MessageViewModel model;
            if (emailCode.ValidUntil < DateTime.Now)
            {
                model = new MessageViewModel()
                {
                    Title = "Link expired",
                    MessageTitle = "Account deletion link expired",
                    MessageContent = "To delete your account request another confirmation email"
                };
                return View("Message", model);
            }

            // Removes account from database and signs out user
            user.Deleted = true;
            await _repository.SaveChangesAsync();
            await HttpContext.SignOutAsync();

            // Returns view
            model = new MessageViewModel()
            {
                Title = "Account deleted",
                MessageTitle = "Account deleted"
            };
            return View("Message", model);
        }
    }
}