using System.Collections;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SimpleForum.Common.Server;
using SimpleForum.Models;

namespace SimpleForum.Web.Policies
{
    public class CheckPassword : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _dbContext;

        public CheckPassword(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectResult("/");
                return;
            }

            string password = (context.HttpContext.Request.Method == HttpMethods.Post) switch
            {
                true => context.HttpContext.Request.Form["password"],
                false => context.HttpContext.Request.Query["password"]
            };

            var viewData = ((Controller)context.Controller).ViewData;
            IEnumerable postData = (context.HttpContext.Request.Method == HttpMethods.Post) switch
            {
                true => context.HttpContext.Request.Form,
                false => context.HttpContext.Request.Query
            };
            if (password == null)
            {
                context.Result = new ViewResult()
                {
                    ViewName = "ConfirmPassword",
                    ViewData = new ViewDataDictionary(viewData)
                    {
                        {"Incorrect", false},
                        {"PostData", postData}
                    }
                };
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
                context.Result = new RedirectResult("/");
                return;
            }

            if (password != user.Password)
            {
                context.Result = new ViewResult()
                {
                    ViewName = "ConfirmPassword",
                    ViewData = new ViewDataDictionary(viewData)
                    {
                        {"Incorrect", true},
                        {"PostData", postData}
                    }
                };
            }
        }
    }
}