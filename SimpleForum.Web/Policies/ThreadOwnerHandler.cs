using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Policies
{
    public class ThreadOwnerHandler : AuthorizationHandler<ThreadOwnerRequirement>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ThreadOwnerHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ThreadOwnerRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated) return Task.CompletedTask;

            int threadID;
            try
            {
                threadID = (_httpContextAccessor.HttpContext.Request.Method == HttpMethods.Post) switch
                {
                    true => int.Parse(_httpContextAccessor.HttpContext.Request.Form["id"]),
                    false => int.Parse(_httpContextAccessor.HttpContext.Request.Query["id"])
                };
            }
            catch
            {
                return Task.CompletedTask;
            }

            Thread thread = _context.Threads.First(x => x.ThreadID == threadID);
            if (thread.UserID.ToString() == context.User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                context.Succeed(requirement);
            }
            
            return Task.CompletedTask;
        }
    }
}