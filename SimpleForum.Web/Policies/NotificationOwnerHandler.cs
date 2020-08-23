using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Policies
{
    public class NotificationOwnerHandler : AuthorizationHandler<NotificationOwnerRequirement>
    {
        // Injects dependencies
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContext;

        public NotificationOwnerHandler(ApplicationDbContext dbContext, IHttpContextAccessor httpContext)
        {
            _dbContext = dbContext;
            _httpContext = httpContext;
        }
        
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotificationOwnerRequirement requirement)
        {
            // returns if user isn't authenticated
            if (!context.User.Identity.IsAuthenticated) return;

            // Retrieves notification id of the request
            int notificationID = (_httpContext.HttpContext.Request.Method == HttpMethods.Post) switch
            {
                true => int.Parse(_httpContext.HttpContext.Request.Form["id"]),
                false => int.Parse(_httpContext.HttpContext.Request.Query["id"])
            };

            // Checks if the notification is for the current user and succeeds requirement if so
            Notification notification = await _dbContext.Notifications.FindAsync(notificationID);

            if (notification.UserID.ToString() == context.User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                context.Succeed(requirement);
            }
        }
    }
}