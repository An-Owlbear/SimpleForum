using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SimpleForum.Models;

namespace SimpleForum.Common.Server.Policies
{
    public class RolesAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        private readonly ApplicationDbContext _context;

        public RolesAuthorizationHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated) return Task.CompletedTask;

            IEnumerable<string> roles = requirement.AllowedRoles;
            User user = _context.Users.First(x =>
                x.UserID.ToString() == context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (roles.Contains(user.Role)) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
