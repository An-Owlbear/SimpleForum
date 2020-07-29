using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Policies
{
    public class OwnerHandler : AuthorizationHandler<OwnerRequirement>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OwnerHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated) return Task.CompletedTask;

            int threadID;
            try
            {
                threadID = int.Parse(_httpContextAccessor.HttpContext.Request.Query["id"]);
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