using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;

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
                int userID = 0;
                bool userBanned = false;
                try
                {
                    userID = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    userBanned = dbContext.Users.First(x => x.UserID == userID).Banned;
                }
                catch
                {
                    // TODO - only ignore certain errors
                    // ignored
                }

                if (userBanned)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    httpContext.Response.ContentType = "text/html";
                    // TODO - Create object containing message title and contents
                    string response = await renderService.RenderToStringAsync("Message", new object());
                    await httpContext.Response.WriteAsync(response);
                    return;
                }
            }
            
            await _next(httpContext);
        }
    }
}