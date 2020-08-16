using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Internal;
using SimpleForum.Models;
using SimpleForum.Web.Models;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private int CommentsPerPage = 15;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
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
                    .Where(x => !x.Deleted)
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
            
            UserComment comment = new UserComment()
            {
                Content = content,
                DatePosted = DateTime.Now,
                UserID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                UserPageID = userPageID,
            };

            await _context.UserComments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return Redirect("/User?id=" + userPageID);
        }

        [Authorize]
        public IActionResult CommentSettings()
        {
            ViewData["User"] =
                _context.Users.First(x => x.UserID == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return View();
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

            ViewData["User"] = user;
            
            return View();
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

            ViewData["User"] = user;
            return View();
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

            ViewData["User"] = user;
            return View();
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(string email, string password, string confirmPassword, string bio)
        {
            if (password != confirmPassword) return RedirectToAction("Edit");
            
            User user;
            try
            {
                int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                user = await _context.Users.FindAsync(userID);
            }
            catch
            {
                return new BadRequestResult();
            }

            if (email != null) user.Email = email;
            if (password != null) user.Password = password;
            if (bio != null) user.Bio = bio;
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", new {id = user.UserID});
        }
    }
}