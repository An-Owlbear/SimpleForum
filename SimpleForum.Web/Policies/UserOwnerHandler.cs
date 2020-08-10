using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace SimpleForum.Web.Policies
{
    public class UserOwnerHandler : AuthorizationHandler<UserOwnerRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserOwnerHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOwnerRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated) return Task.CompletedTask;

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

            if (context.User.FindFirstValue(ClaimTypes.NameIdentifier) == userID)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}