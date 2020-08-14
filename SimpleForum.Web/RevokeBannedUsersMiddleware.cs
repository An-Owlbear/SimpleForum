using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;
using SimpleForum.Models;
using SimpleForum.Web.Models;

namespace SimpleForum.Web
{
    public class RevokeBannedUsersMiddleware
    {
        private readonly RequestDelegate _next;

        public RevokeBannedUsersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext dbContext, IViewRenderService renderService)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                User user;
                try
                {
                    int userID = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    user = dbContext.Users.First(x => x.UserID == userID);
                }
                catch
                {
                    MessageViewModel model = new MessageViewModel()
                    {
                        Title = "Error",
                        MessageTitle = "Error during request",
                        MessageContent = "Clear your cookies to resolve the issue."
                    };
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    httpContext.Response.ContentType = "text/html";
                    string response = await renderService.RenderToStringAsync("Message", model);
                    await httpContext.Response.WriteAsync(response);
                    return;
                }

                if (user.Banned)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    httpContext.Response.ContentType = "text/html";
                    
                    MessageViewModel model = new MessageViewModel()
                    {
                        Title = "Account banned",
                        MessageTitle = "Your account is banned",
                        MessageContent = "Ban reason: " + user.BanReason
                    };
                    
                    string response = await renderService.RenderToStringAsync("Message", model);
                    await httpContext.Response.WriteAsync(response);
                    return;
                }
            }
            
            await _next(httpContext);
        }
    }
}