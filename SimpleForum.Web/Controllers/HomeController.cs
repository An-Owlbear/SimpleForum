using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Web.Models;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Finds the replies for a given thread
            int replies(Thread thread)
            {
                return (from comment in _context.Comments
                    where comment.ThreadID == thread.ThreadID
                    select comment).Count();
            }

            // Creates a list of threads and replies and returns the view
            IEnumerable<(Thread, int)> threads = _context.Threads.ToList().Select(x => (x, replies(x)));
            ViewData["Threads"] = threads;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}