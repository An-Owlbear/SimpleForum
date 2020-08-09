using System.Security.Claims;
using SimpleForum.Models;

namespace SimpleForum.Web.Views
{
    public static class RazorMethods
    {
        public static bool IsOwner(ClaimsPrincipal user, IPost post)
        {
            if (!user.Identity.IsAuthenticated) return false;
            try
            {
                if (int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)) == post.UserID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}