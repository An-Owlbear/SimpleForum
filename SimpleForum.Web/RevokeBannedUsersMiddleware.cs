using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext dbContext)
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
                    await httpContext.SignOutAsync();
                    httpContext.Response.Redirect("/Home/Banned?userID=" + userID);
                }
            }
            
            await _next(httpContext);
        }
    }
}