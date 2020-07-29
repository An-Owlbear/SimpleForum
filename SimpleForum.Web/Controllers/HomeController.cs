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
        private int ThreadsPerPage = 30;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            // Creates a list of threads and replies and returns the view
            IEnumerable<(Thread, int)> threads = _context.Threads.Where(x =>x.Deleted == false)
                .OrderByDescending(x => x.DatePosted).Skip((page - 1) * ThreadsPerPage).Take(ThreadsPerPage)
                .ToList().Select(x => (x, x.Comments.Count));
            
            ViewData["Threads"] = threads;
            ViewData["Page"] = page;
            ViewData["PageCount"] = (_context.Threads.ToList().Count + (ThreadsPerPage - 1)) / ThreadsPerPage;
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

        public IActionResult Forbidden()
        {
            ViewData["Title"] = "Forbidden";
            ViewData["MessageTitle"] = "Access denied.";
            return View("Message");
        }
    }
}