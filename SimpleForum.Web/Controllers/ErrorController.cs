using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Internal;
using SimpleForum.Models;
using SimpleForum.Web.Models;

namespace SimpleForum.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ErrorController(ApplicationDbContext context)
        {
            _context = context;
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

        public IActionResult AccessDenied()
        {
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Access denied",
                MessageTitle = "Access denied"
            };
            return View("Message", model);
        }

        [Authorize]
        public async Task<IActionResult> Banned()
        {
            User user;
            try
            {
                int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                user = await _context.Users.FindAsync(userID);
            }
            catch
            {
                return BadRequest();
            }

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Account banned",
                MessageTitle = "Your account is banned",
                MessageContent = "Ban reasosn: " + user.BanReason
            };

            return View("Message", model);
        }
    }
}