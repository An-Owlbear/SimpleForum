using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
        private readonly ApplicationDbContext _context;
        private readonly SimpleForumRepository _repository;
        private readonly SimpleForumConfig _config;
        private readonly IEmailService _emailService;
        private int CommentsPerPage = 15;

        public UserController(ApplicationDbContext context, IOptions<SimpleForumConfig> config, IEmailService emailService, SimpleForumRepository repository)
        {
            _context = context;
            _config = config.Value;
            _emailService = emailService;
            _repository = repository;
        }
        
        public IActionResult Index(int? id, int page = 1)
        {
            if (id == null) return Redirect("/");
            User user;

            try
            {
                user = _context.Users.First(x => x.UserID == id);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(404);
            }
            
            // Returns error if user is deleted
            if (user.Deleted)
            {
                MessageViewModel messageViewModel = new MessageViewModel()
                {
                    Title = "User deleted",
                    MessageTitle = "User deleted"
                };
                return View("Message", messageViewModel);
            }

            User currentUser = null;
            if (User.Identity.IsAuthenticated)
            {
                int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                currentUser = _context.Users.First(x => x.UserID == userID);
            }
            
            UserPageViewModel model = new UserPageViewModel()
            {
                User = user,
                Page = page,
                PageCount = (user.UserPageComments.Count(x => !x.Deleted) + (CommentsPerPage - 1)) / CommentsPerPage,
                CurrentUser = currentUser,
                CurrentPageComments = user.UserPageComments
                    .Where(x => !x.Deleted && !x.User.Deleted)
                    .OrderByDescending(x => x.DatePosted)
                    .Skip((page - 1) * CommentsPerPage)
                    .Take(CommentsPerPage)
            };
            
            return View("User", model);
        }

        [Authorize(Policy = "UserPageReply")]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(PreventMuted))]
        public async Task<IActionResult> PostUserComment(string content, int userPageID)
        {
            if (content == null) return Redirect("/");
            int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = await _context.Users.FindAsync(userID);
            
            UserComment comment = new UserComment()
            {
                Content = content,
                DatePosted = DateTime.Now,
                UserID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                UserPageID = userPageID,
            };
            await _context.UserComments.AddAsync(comment);
            
            // Creates a notification if commenting on another user
            if (userID != userPageID)
            {
                Notification notification = new Notification()
                {
                    Title = $"{user.Username} left a comment on your profile",
                    DateCreated = DateTime.Now,
                    UserID = userPageID
                };
                await _context.Notifications.AddAsync(notification);
            }
            
            await _context.SaveChangesAsync();
            return Redirect("/User?id=" + userPageID);
        }

        [Authorize]
        [HttpPost]
        [ServiceFilter(typeof(CheckPassword))]
        // Deletes a posted UserComment
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        // Returns the form for deleting a comment as an admin
        public async Task<IActionResult> AdminDeleteUserComment(int userCommentID)
        {
            UserComment comment = await _repository.GetUserCommentAsync(userCommentID);
            return View(comment);
        }
        
        [Authorize]
        [HttpPost]
        [ServiceFilter(typeof(CheckPassword))]
        // Deletes a UserComment as an admin
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

        [Authorize]
        public async Task<IActionResult> CommentSettings()
        {
            int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = await _context.Users.FindAsync(userID);
            return View(user);
        }

        [Authorize(Policy = "UserOwner")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> LockComments(int? id)
        {
            if (id == null) return Redirect("/");
            
            User user = _context.Users.First(x => x.UserID == id);

            user.CommentsLocked = true;
            await _context.SaveChangesAsync();
            
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comments locked",
                MessageContent = "Comments locked"
            };
            return View("Message", model);
        }

        [Authorize(Policy = "UserOwner")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> ClearComments(int? id)
        {
            if (id == null) return Redirect("/");

            User user = _context.Users.First(x => x.UserID == id);
            
            foreach (UserComment userComment in user.UserPageComments)
            {
                userComment.Deleted = true;
            }

            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comments cleared",
                MessageTitle = "Comments cleared"
            };
            return View("Message", model);
        }

        [Authorize(Policy = "UserOwner")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> UnlockComments(int? id)
        {
            if (id == null) return Redirect("/");

            User user = _context.Users.First(x => x.UserID == id);
            user.CommentsLocked = false;
            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comments unlocked",
                MessageTitle = "Comments unlocked"
            };
            return View("Message", model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminActions(int? id)
        {
            if (id == null) return Redirect("/");

            User user;
            try
            {
                user = _context.Users.First(x => x.UserID == id);
            }
            catch
            {
                return Redirect("/");
            }
            
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult MuteUser(int? id)
        {
            if (id == null) return Redirect("/");

            User user;
            try
            {
                user = _context.Users.First(x => x.UserID == id);
            }
            catch
            {
                return Redirect("/");
            }
            
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendMuteUser(int? id, string reason)
        {
            if (id == null || reason == null) return Redirect("/");

            User user;
            try
            {
                user = _context.Users.First(x => x.UserID == id);
            }
            catch
            {
                return Redirect("/");
            }

            user.Muted = true;
            user.MuteReason = reason;
            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "User muted",
                MessageTitle = "User muted"
            };
            return View("Message", model);
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendUnmuteUser(int? id)
        {
            if (id == null) return Redirect("/");

            User user;
            try
            {
                user = _context.Users.First(x => x.UserID == id);
            }
            catch
            {
                return Redirect("/");
            }

            user.Muted = false;
            user.MuteReason = null;
            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "User unmuted",
                MessageTitle = "User unmuted"
            };
            return View("Message", model);
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult BanUser(int? id)
        {
            if (id == null) return Redirect("/");

            User user;
            try
            {
                user = _context.Users.First(x => x.UserID == id);
            }
            catch
            {
                return Redirect("/");
            }
            
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendBanUser(int? id, string reason)
        {
            if (id == null || reason == null) return Redirect("/");

            User user;
            try
            {
                user = _context.Users.First(x => x.UserID == id);
            }
            catch
            {
                return Redirect("/");
            }

            user.Banned = true;
            user.BanReason = reason;
            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "User banned",
                MessageTitle = "User banned"
            };
            return View("Message", model);
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendUnbanUser(int? id)
        {
            if (id == null) return Redirect("/");

            User user;
            try
            {
                user = _context.Users.First(x => x.UserID == id);
            }
            catch
            {
                return Redirect("/");
            }

            user.Banned = false;
            user.BanReason = null;
            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "User unbanned",
                MessageTitle = "User unbanned"
            };
            return View("Message", model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit()
        {
            User user;
            try
            {
                int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                user = _context.Users.First(x => x.UserID == userID);
            }
            catch
            {
                return new BadRequestResult();
            }
            
            EditUserViewModel model = new EditUserViewModel()
            {
                User = user
            };

            return View(model);
        }

        // Updates a user's account information
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(string email, string password, string confirmPassword, string bio,
            IFormFile profilePicture)
        {
            // Returns if submitted passwords do not match
            if (password != confirmPassword) return RedirectToAction("Edit");
            
            // Retrieves user account
            User user;
            try
            {
                int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                user = await _context.Users.FindAsync(userID);
            }
            catch { return new BadRequestResult(); }

            // Changes information where applicable
            if (email != null) user.Email = email;
            if (password != null) user.Password = password;
            if (bio != null) user.Bio = bio;
            await _context.SaveChangesAsync();

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

        [Authorize]
        public async Task<IActionResult> Notifications()
        {
            int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = await _context.Users.FindAsync(userID);
            
            NotificationsViewModel model = new NotificationsViewModel()
            {
                Notifications = user.Notifications.OrderByDescending(x => x.DateCreated)
            };

            return View(model);
        }

        [Authorize(Policy = "NotificationOwner")]
        public async Task<IActionResult> Notification(int? id)
        {
            // Redirects if id parameter is null
            if (id == null) return Redirect("/");

            // Retrieves notification from database and sets to read if unread
            Notification notification = await _context.Notifications.FindAsync(id);
            if (!notification.Read)
            {
                notification.Read = true;
                await _context.SaveChangesAsync();
            }
            
            // Returns view with notification
            return View(notification);
        }

        [Authorize]
        [ServiceFilter(typeof(CheckPassword))]
        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            // Creates a code to be used for the email
            DateTime now = DateTime.Now;
            User user = await _context.Users.FindAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            EmailCode code = new EmailCode()
            {
                Code = Tools.GenerateCode(32),
                DateCreated = now,
                Type = "AccountDelete",
                User = user,
                ValidUntil = now.AddHours(1)
            };
            await _context.EmailCodes.AddAsync(code);
            await _context.SaveChangesAsync();
            
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
            
            // Returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Delete account",
                MessageTitle = "Delete account",
                MessageContent = "A confirmation email has been sent to your email account. This email is valid for oen hour."
            };
            return View("Message", model);
        }

        [Authorize]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendDelete(string code)
        {
            // Redirects to index if code is null
            if (code == null) return Redirect("/");
            
            // Retrieves user and email and verified the correct user is signed in.
            User user = await _context.Users.FindAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            EmailCode emailCode = await _context.EmailCodes.FindAsync(code);
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
            await _context.SaveChangesAsync();
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