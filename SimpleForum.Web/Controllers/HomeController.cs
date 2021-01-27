using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleForum.Web.Models;
using SimpleForum.Common.Server;
using SimpleForum.Models;

namespace SimpleForum.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly SimpleForumRepository _repository;
        private readonly SimpleForumConfig _config;

        public HomeController(SimpleForumRepository repository, IOptionsSnapshot<SimpleForumConfig> config)
        {
            _repository = repository;
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

        public IActionResult Privacy()
        {
            return View();
        }
    }
}