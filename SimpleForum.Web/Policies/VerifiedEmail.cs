using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using SimpleForum.Common.Server;
using SimpleForum.Models;
using SimpleForum.Web.Models;

namespace SimpleForum.Web.Policies
{
    public class VerifiedEmail : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;
        private readonly SimpleForumConfig _config;

        public VerifiedEmail(ApplicationDbContext context, IOptionsSnapshot<SimpleForumConfig> config)
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
                string resendUrl = _config.InstanceURL + "/Signup/ResendVerificationEmail?userID=" + user.UserID;
                MessageViewModel model = new MessageViewModel()
                {
                    Title = "Forbidden",
                    MessageTitle = "Your email is not verified",
                    MessageContent = "To access this page your email account must be verified. We have sent you" +
                                     " an email containing the verification link. If you have not received the email" +
                                     $" click [here]({resendUrl})."
                };
                ViewDataDictionary viewData = ((Controller)context.Controller).ViewData;
                context.Result = new ViewResult()
                {
                    ViewName = "Message",
                    ViewData = new ViewDataDictionary(viewData)
                    {
                        Model = model
                    }
                };
            }
        }
    }
}