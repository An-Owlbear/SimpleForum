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
            IEnumerable<(Thread, int)> threads = _context.Threads
                .Where(x =>x.Deleted == false)
                .OrderByDescending(x => x.Pinned)
                .ThenByDescending(x => x.DatePosted)
                .Skip((page - 1) * ThreadsPerPage)
                .Take(ThreadsPerPage)
                .ToList().Select(x => (x, x.Comments.Count));

            // Checks if the user's email is verified and sets EmailVerified accordingly
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    User user = _context.Users.First(x =>
                        x.UserID.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier));
                    ViewData["EmailVerified"] = user.Activated;
                    ViewData["EmailVerifyRequired"] = _config.RequireEmailVerification;
                }
                catch
                {
                    ViewData["EmailVerified"] = false;
                }
            }
            
            // Sets thread and page variables and returns view
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

        public IActionResult EmailUnverified()
        {
            string resendUrl = _config.InstanceURL + "/Signup/ResendVerificationEmail?userID=" +
                               User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["Title"] = "Forbidden";
            ViewData["MessageTitle"] = "Your email is not verified.";
            ViewData["MessageContent"] = "To access this page your email account must be verified. We have sent you" +
                                         " an email containing the verification link. If you have not received the email" +
                                         $" click [here]({resendUrl}).";
            return View("Message");
        }

        public IActionResult Banned(int? userID)
        {
            ViewData["Title"] = ViewData["MessageTitle"] = "Your account is banned";
            ViewData["MessageContent"] = "Ban reason: " + _context.Users.First(x => x.UserID == userID).BanReason;
            return View("Message");
        }
    }
}