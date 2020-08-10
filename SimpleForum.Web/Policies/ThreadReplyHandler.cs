using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Policies
{
    public class ThreadReplyHandler : AuthorizationHandler<ThreadReplyRequirement>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ThreadReplyHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ThreadReplyRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated) return Task.CompletedTask;

            int threadID;
            try
            {
                threadID = (_httpContextAccessor.HttpContext.Request.Method == HttpMethods.Post) switch
                {
                    true => int.Parse(_httpContextAccessor.HttpContext.Request.Form["threadID"]),
                    false => int.Parse(_httpContextAccessor.HttpContext.Request.Query["id"])
                };
            }
            catch
            {
                return Task.CompletedTask;
            }

            Thread thread = _context.Threads.First(x => x.ThreadID == threadID);
            if (!thread.Locked) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}