using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Internal;
using SimpleForum.Models;

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
        
        // GET
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
            ViewData["PageComments"] = user.UserPageComments.OrderByDescending(x => x.DatePosted)
                .Skip((page - 1) * CommentsPerPage).Take(CommentsPerPage);
            ViewData["Page"] = page;
            ViewData["PageCount"] = (user.UserPageComments.Count + (CommentsPerPage - 1)) / CommentsPerPage;

            return View("User");
        }

        public async Task<IActionResult> PostUserComment(string content, int userPageID)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Login");
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
    }
}