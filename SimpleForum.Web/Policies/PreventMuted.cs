using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SimpleForum.Common.Server;
using SimpleForum.Models;
using SimpleForum.Web.Models;

namespace SimpleForum.Web.Policies
{
    public class PreventMuted : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _dbContext;

        public PreventMuted(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
                return;
            }

            User user;
            try
            {
                user = _dbContext.Users.First(x =>
                    x.UserID == int.Parse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
            }
            catch
            {
                context.Result = new NotFoundResult();
                return;
            }

            if (user.Muted)
            {
                var viewData = ((Controller)context.Controller).ViewData;
                MessageViewModel model = new MessageViewModel()
                {
                    Title = "Access denied",
                    MessageTitle = "Your account is muted, you cannot make further posts",
                    MessageContent = "Reason: " + user.MuteReason
                };
                
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