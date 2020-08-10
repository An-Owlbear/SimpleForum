using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Policies
{
    public class CommentOwnerRequirement : IAuthorizationRequirement {}
    
    public class CommentOwnerHandler : AuthorizationHandler<CommentOwnerRequirement>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentOwnerHandler(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CommentOwnerRequirement requirement)
        {
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated) return Task.CompletedTask;

            int commentID;
            Comment comment; 
            try
            {
                commentID = (_httpContextAccessor.HttpContext.Request.Method == HttpMethods.Post) switch
                {
                    true => int.Parse(_httpContextAccessor.HttpContext.Request.Form["id"]),
                    false => int.Parse(_httpContextAccessor.HttpContext.Request.Query["id"])
                };
                comment = _dbContext.Comments.First(x => x.CommentID == commentID);
            }
            catch
            {
                return Task.CompletedTask;
            }

            if (int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))
                == comment.UserID)
            {
                context.Succeed(requirement);
            }
            
            return Task.CompletedTask;
        }
    }
}