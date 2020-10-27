using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;

namespace SimpleForum.Web.Policies
{
    public class UserOwnerOrAdminHandler : AuthorizationHandler<UserOwnerOrAdminRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public UserOwnerOrAdminHandler(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOwnerOrAdminRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated) return Task.CompletedTask;
            if (_context.Users.First(x => x.UserID.ToString() == context.User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Role == "Admin")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            string userID;
            try
            {
                userID = (_httpContextAccessor.HttpContext.Request.Method == HttpMethods.Post) switch
                {
                    true => _httpContextAccessor.HttpContext.Request.Form["id"],
                    false => _httpContextAccessor.HttpContext.Request.Query["id"]
                };
            }
            catch
            {
                return Task.CompletedTask;
            }
            
            if (userID == context.User.FindFirstValue(ClaimTypes.NameIdentifier)) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}