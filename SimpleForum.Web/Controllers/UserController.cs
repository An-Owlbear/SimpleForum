using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET
        public IActionResult Index(int? id)
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
            ViewData["PageComments"] = user.UserPageComments.OrderByDescending(x => x.DatePosted);
            return View("User");
        }
    }
}