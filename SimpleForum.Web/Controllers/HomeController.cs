using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleForum.Web.Models;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SimpleForumConfig _config;
        private int ThreadsPerPage = 30;

        public HomeController(ApplicationDbContext context, IOptions<SimpleForumConfig> config)
        {
            _context = context;
            _config = config.Value;
        }

        public IActionResult Index(int page = 1)
        {
            // Creates a list of threads and replies and returns the view
            IEnumerable<Thread> threads = _context.Threads
                .Where(x => x.Deleted == false)
                .OrderByDescending(x => x.Pinned)
                .ThenByDescending(x => x.DatePosted)
                .Skip((page - 1) * ThreadsPerPage)
                .Take(ThreadsPerPage).ToList();

            // Checks if the user's email is verified and sets EmailVerified accordingly
            bool emailVerified = false;
            bool emailVerificationRequired = _config.RequireEmailVerification;
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User user = _context.Users.First(x => x.UserID == userID);
                    emailVerified = user.Activated;
                }
                catch
                {
                    emailVerified = false;
                }
            }
            
            // Sets thread and page variables and returns view
            IndexViewModel model = new IndexViewModel()
            {
                Threads = threads,
                Page = page,
                PageCount = (_context.Threads.Count(x => !x.Deleted) + (ThreadsPerPage - 1)) / ThreadsPerPage,
                EmailVerified = emailVerified,
                EmailVerificationRequired = emailVerificationRequired
            };
            return View(model);
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

        public IActionResult StatusError(int code)
        {
            MessageViewModel model = new MessageViewModel()
            {
                Title = $"Error {code}",
                MessageTitle = $"Error {code}"
            };
            return View("Message", model);
        }
    }
}