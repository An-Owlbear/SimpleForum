using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleForum.Common;
using SimpleForum.Web.Models;
using SimpleForum.Common.Server;
using SimpleForum.Models;

namespace SimpleForum.Web.Controllers
{
    public class HomeController : WebController
    {
        private readonly SimpleForumRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly SimpleForumConfig _config;

        public HomeController(SimpleForumRepository repository, ApplicationDbContext context, IOptionsSnapshot<SimpleForumConfig> config)
        {
            _repository = repository;
            _context = context;
            _config = config.Value;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            // Retrieves the threads at the front page
            IEnumerable<Thread> threads = await _repository.GetFrontPageAsync(page);

            // Checks if the user's email is verified and sets EmailVerified accordingly
            bool emailVerified = false;
            bool emailVerificationRequired = _config.RequireEmailVerification;
            if (User.Identity.IsAuthenticated)
            {
                User user = await _repository.GetUserAsync(User);
                emailVerified = user.Activated;
            }
            
            // Sets thread and page variables and returns view
            IndexViewModel model = new IndexViewModel()
            {
                Threads = threads,
                Page = page,
                PageCount = await _repository.GetPageCountAsync(),
                EmailVerified = emailVerified,
                EmailVerificationRequired = emailVerificationRequired
            };
            return View(model);
        }

        // Returns the privacy policy screen
        public IActionResult Privacy()
        {
            string privacyPolicy = System.IO.File.ReadAllText("../Data/privacy.txt");
            MessageViewModel model = new MessageViewModel()
            {
                MessageTitle = "Privacy Policy",
                MessageContent = privacyPolicy
            };
            return View("Message", model);
        }

        // Returns the terms and conditions screen
        public IActionResult Terms()
        {
            string terms = System.IO.File.ReadAllText("../Data/terms.txt");
            MessageViewModel model = new MessageViewModel()
            {
                MessageTitle = "Terms and Conditions",
                MessageContent = terms
            };
            return View("Message", model);
        }

        // Returns instance information as json
        public IActionResult InstanceInfo()
        {
            return Json(new
            {
                InstanceURL = _config.InstanceURL,
                APIURL = _config.APIURL,
                CrossConnectionURL = _config.CrossConnectionURL
            });
        }

        // Returns the form for settings up the first user of the forum
        public IActionResult Setup(int? error)
        {
            if (_context.Users.Count() != 0) return Redirect("/");
            FormViewModel model = new FormViewModel()
            {
                Error = error
            };
            return View(model);
        }

        // Sets up the first user of the forum
        public async Task<IActionResult> SendSetup(string email, string username, string password, string confirmPassword)
        {
            // Redirects if users have already signed up or input is invalid
            if (_context.Users.Count() != 0) return Redirect("/");
            if (email == null || username == null || password == null) return RedirectToAction("Setup", new { error = 0 });
            if (password != confirmPassword) return RedirectToAction("Setup", new { error = 1 });

            User user = new User()
            {
                Username = username,
                Email = email,
                Password = password,
                Role = "Admin",
                Activated = true
            };
            User addedUser = await _repository.AddUserAsync(user);
            
            // Saves changes
            await _repository.SaveChangesAsync();
            
            // Signs in user
            ClaimsPrincipal principal = Auth.CreateClaims(user);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMonths(1),
                    IsPersistent = true,
                    AllowRefresh = false
                });

            return Redirect("/");
        }
    }
}