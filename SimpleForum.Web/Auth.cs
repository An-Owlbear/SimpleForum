using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using SimpleForum.Models;

namespace SimpleForum.Web
{
    public static class Auth
    {
        // Creates ClaimsIdentity for the give user
        public static ClaimsPrincipal CreateClaims(User user)
        {
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            return new ClaimsPrincipal(identity);
        }
    }
}