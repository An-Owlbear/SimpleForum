using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;
using SimpleForum.Models;
using Thread = SimpleForum.Models.Thread;

namespace SimpleForum.Web.Policies
{
    public class ThreadOwnerOrAdminHandler : AuthorizationHandler<ThreadOwnerOrAdminRequirement>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ThreadOwnerOrAdminHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ThreadOwnerOrAdminRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated) return Task.CompletedTask;
            User user = _context.Users.First(x =>
                x.UserID.ToString() == context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user.Role == "Admin") context.Succeed(requirement);
            
            int ThreadID;
            try
            {
                ThreadID = int.Parse(_httpContextAccessor.HttpContext.Request.Query["id"]);
            }
            catch
            {
                return Task.CompletedTask;
            }

            Thread thread = _context.Threads.First(x => x.ThreadID == ThreadID);
            if (thread.UserID.ToString() == context.User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}