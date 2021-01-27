using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SimpleForum.Common.Server;
using SimpleForum.Models;

namespace SimpleForum.Web
{
    public class RevokeUsersMiddleware
    {
        private readonly RequestDelegate _next;

        public RevokeUsersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, SimpleForumRepository repository, IViewRenderService renderService)
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
                User user = await repository.GetUserAsync(httpContext.User);

                if (user.Deleted)
                {
                    await httpContext.SignOutAsync();
                }
                else if (user.Banned)
                {
                    httpContext.Response.Redirect("/Error/Banned");
                }
            }
            
            await _next(httpContext);
        }
    }
}