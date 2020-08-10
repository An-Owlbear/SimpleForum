using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Policies
{
    public class UserPageReplyHandler : AuthorizationHandler<UserPageReplyRequirement>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserPageReplyHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserPageReplyRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated) return Task.CompletedTask;

            User user;
            try
            {
                int userID = (_httpContextAccessor.HttpContext.Request.Method == HttpMethods.Post) switch
                {
                    true => int.Parse(_httpContextAccessor.HttpContext.Request.Form["userPageID"]),
                    false => int.Parse(_httpContextAccessor.HttpContext.Request.Query["userPageID"])
                };
                user = _context.Users.First(x => x.UserID == userID);
            }
            catch
            {
                return Task.CompletedTask;
            }
            
            if (!user.CommentsLocked) context.Succeed(requirement);
            
            return Task.CompletedTask;
        }
    }
}