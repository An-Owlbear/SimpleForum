using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleForum.Internal;

namespace SimpleForum.Web
{
    public static class StartupExtensions
    {
        public static IApplicationBuilder UseRevokeBannedUsers(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RevokeBannedUsersMiddleware>();
        }
    }
}