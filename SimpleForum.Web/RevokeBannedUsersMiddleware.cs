using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;
using SimpleForum.Models;

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
            // Urls which can be accessed whilst an account is banned
            List<string> urlExceptions = new List<string>()
            {
                "/Error/Banned",
                "/Login/Logout",
                "/Error/StatusError"
            };
            
            List<bool> conditions = new List<bool>()
            {
                httpContext.User.Identity.IsAuthenticated,
                urlExceptions.All(x => x != httpContext.Request.Path.Value)
            };
            
            if (conditions.All(x => x))
            {
                User user;
                int userID = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                user = dbContext.Users.First(x => x.UserID == userID);
                

                if (user.Banned)
                {
                    httpContext.Response.Redirect("/Error/Banned");
                }
            }
            
            await _next(httpContext);
        }
    }
}