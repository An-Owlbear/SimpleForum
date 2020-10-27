using Microsoft.AspNetCore.Builder;

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