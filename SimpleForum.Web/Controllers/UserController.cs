using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NETCore.MailKit.Core;
using SimpleForum.Internal;
using SimpleForum.Models;
using SimpleForum.Web.Models;
using SimpleForum.Web.Policies;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SimpleForum.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly SimpleForumRepository _repository;
        private readonly SimpleForumConfig _config;
        private readonly IEmailService _emailService;
        private int CommentsPerPage = 15;

        public UserController(IOptions<SimpleForumConfig> config, IEmailService emailService, SimpleForumRepository repository)
        {
            _config = config.Value;
            _emailService = emailService;
            _repository = repository;
        }
        
        // Returns a user's profile
        public async Task<IActionResult> Index(int id, int page = 1)
        {
            // Retrieves users and comments
            User user = await _repository.GetUserAsync(id);
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
        [Authorize(Policy = "UserPageReply")]
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
            await _repository.PostUserCommentAsync(comment);
            await _repository.SaveChangesAsync();
            
            // Redirects to posted comment
            return Redirect("/User?id=" + userPageID);
        }

        // Deletes a posted UserComment
        [Authorize]
        [HttpPost]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> DeleteUserComment(int userCommentID)
        {
            // Deletes the comment and returns 403 in not authorised
            try
            {
                await _repository.DeleteUserCommentAsync(userCommentID, User);
                await _repository.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                return Forbid();
            }
            
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
            try
            {
                await _repository.AdminDeleteUserCommentAsync(userCommentID, reason);
                await _repository.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            
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
        [Authorize(Policy = "UserOwner")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> LockComments(int id)
        {
            // Retrieves user and sets comments as locked
            User user = await _repository.GetUserAsync(id);
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
        [Authorize(Policy = "UserOwner")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> ClearComments(int id)
        {
            // Retrieves user and deletes comments
            User user = await _repository.GetUserAsync(id);
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
        [Authorize(Policy = "UserOwner")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> UnlockComments(int id)
        {
            // Retrieves users and locks comments
            User user = await _repository.GetUserAsync(id);
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
            
            // Retrieves user account and changes information where applicable
            User user = await _repository.GetUserAsync(User);
            if (email != null) user.Email = email;
            if (password != null) user.Password = password;
            if (bio != null) user.Bio = bio;
            await _repository.SaveChangesAsync();

            // Updates profile picture
            await using (MemoryStream outputImage = new MemoryStream())
            {
                if (profilePicture != null)
                {
                    // Creates image object to edit
                    
                    Image imageObject = await Image.LoadAsync(profilePicture.OpenReadStream());

                    // Calculates values and crops image
                    int diff = Math.Abs(imageObject.Height - imageObject.Width);
                    int crop = (diff + 1) / 2;

                    if (imageObject.Width > imageObject.Height)
                    {
                        imageObject.Mutate(x =>
                            x.Crop(new Rectangle(crop, 0, imageObject.Height, imageObject.Height)));
                    }
                    else if (imageObject.Width < imageObject.Height)
                    {
                        imageObject.Mutate(x =>
                            x.Crop(new Rectangle(0, crop, imageObject.Width, imageObject.Width)));
                    }

                    // Resizes image to 1000x1000 px if greater
                    if (imageObject.Width > 1000)
                        imageObject.Mutate(x => x.Resize(1000, 1000));

                    // Writes image to file
                    await imageObject.SaveAsJpegAsync(outputImage);
                    await System.IO.File.WriteAllBytesAsync(
                        $"UploadedImages/ProfilePictures/{user.UserID}.jpg", outputImage.ToArray());
                }
            }
            
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
        [Authorize(Policy = "NotificationOwner")]
        public async Task<IActionResult> Notification(int id)
        {
            // Retrieves notification from database and sets to read if unread
            Notification notification = await _repository.GetNotificationAsync(id);
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
            // Creates a code to be used for the email
            DateTime now = DateTime.Now;
            User user = await _repository.GetUserAsync(User);
            EmailCode code = new EmailCode()
            {
                Code = Tools.GenerateCode(32),
                DateCreated = now,
                Type = "AccountDelete",
                User = user,
                ValidUntil = now.AddHours(1)
            };
            await _repository.AddEmailCodeAsync(code);
            await _repository.SaveChangesAsync();
            
            // Sends the confirmation email is the user
            string url = _config.InstanceURL + "/User/SendDelete?code=" + code.Code;
            await _emailService.SendAsync(
                user.Email,
                "SimpleForum account deletion confirmation",
                $"<p>Please click the following link to confirm your account deletion: <a href=\"{url}\">{url}</a>" +
                "<br>If you did not try to delete your account please change your password to prevent further unauthorised access" +
                "of your account.</p>",
                true
            );
            
            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Delete account",
                MessageTitle = "Delete account",
                MessageContent = "A confirmation email has been sent to your email account. This email is valid for oen hour."
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