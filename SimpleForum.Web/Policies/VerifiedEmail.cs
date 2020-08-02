using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Policies
{
    public class VerifiedEmail : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;
        private readonly SimpleForumConfig _config;

        public VerifiedEmail(ApplicationDbContext context, IOptions<SimpleForumConfig> config)
        {
            _context = context;
            _config = config.Value;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_config.RequireEmailVerification) return;
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            User user;
            try
            {
                user = _context.Users.First(x =>
                    x.UserID.ToString() == context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            if (!user.Activated)
            {
                context.Result = new RedirectToActionResult("EmailUnverified", "Home", null);
            }
        }
    }
}