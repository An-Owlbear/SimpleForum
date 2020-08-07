using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Internal;
using SimpleForum.Models;
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
                return Redirect("/");
            }

            ViewData["PostCount"] = user.Comments.Count;
            ViewData["Title"] = user.Username;
            ViewData["User"] = user;
            ViewData["PageComments"] = user.UserPageComments
                .Where(x => !x.Deleted)
                .OrderByDescending(x => x.DatePosted)
                .Skip((page - 1) * CommentsPerPage)
                .Take(CommentsPerPage);
            ViewData["Page"] = page;
            ViewData["PageCount"] = (user.UserPageComments.Count + (CommentsPerPage - 1)) / CommentsPerPage;

            if (User.Identity.IsAuthenticated)
            {
                ViewData["Role"] = _context.Users
                    .First(x => x.UserID == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))).Role;
            }
            
            return View("User");
        }

        [Authorize(Policy = "UserPageReply")]
        [ServiceFilter(typeof(VerifiedEmail))]
        public async Task<IActionResult> PostUserComment(string content, int userPageID)
        {
            if (content == null) return Redirect("/");
            
            UserComment comment = new UserComment()
            {
                Content = content,
                DatePosted = DateTime.Now,
                UserID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                UserPageID = userPageID
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
        public async Task<IActionResult> LockComments(int? id)
        {
            if (id == null) return Redirect("/");
            
            User user = _context.Users.First(x => x.UserID == id);

            user.CommentsLocked = true;
            await _context.SaveChangesAsync();

            ViewData["Title"] = ViewData["MessageTitle"] = "Comments locked";
            return View("Message");
        }

        [Authorize(Policy = "UserOwner")]
        public async Task<IActionResult> ClearComments(int? id)
        {
            if (id == null) return Redirect("/");

            User user = _context.Users.First(x => x.UserID == id);
            
            foreach (UserComment userComment in user.UserPageComments)
            {
                userComment.Deleted = true;
            }

            await _context.SaveChangesAsync();

            ViewData["Title"] = ViewData["MessageTitle"] = "Comments cleared";
            return View("Message");
        }

        [Authorize(Policy = "UserOwner")]
        public async Task<IActionResult> UnLockComments(int? id)
        {
            if (id == null) return Redirect("/");

            User user = _context.Users.First(x => x.UserID == id);
            user.CommentsLocked = false;
            await _context.SaveChangesAsync();

            ViewData["Title"] = ViewData["MessageTitle"] = "Comments unlocked";
            return View("Message");
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
;        }

        [Authorize(Roles = "Admin")]
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

            ViewData["Title"] = ViewData["MessageTitle"] = "User muted";
            return View("Message");
        }

        [Authorize(Roles = "Admin")]
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

            ViewData["Title"] = ViewData["MessageTitle"] = "User unmuted";
            return View("Message");
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

            ViewData["Title"] = ViewData["MessageTitle"] = "User banned";
            return View("Message");
        }

        [Authorize(Roles = "Admin")]
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

            ViewData["Title"] = ViewData["MessageTitle"] = "User unbanned";
            return View("Message");
        }
    }
}