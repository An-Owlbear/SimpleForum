using System.Linq;
using System.Numerics;
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
            if (id == null) Redirect("/");

            User user = _context.Users.First(x => x.UserID == id);
            ViewData["PostCount"] = user.Comments.Count;
            ViewData["Title"] = user.Username;
            ViewData["User"] = user;
            return View("User");
        }
    }
}