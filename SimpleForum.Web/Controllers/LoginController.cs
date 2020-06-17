using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            return View("Login");
        }

        public async Task<IActionResult> SendLogin(string username, string password)
        {
            if (username == null || password == null || User.Identity.IsAuthenticated) return Redirect("/");
            User user;

            try
            {
                user = username switch
                {
                    var x when x.Contains("@") => _context.Users.First(y => y.Email == username),
                    var x when !x.Contains("@") => _context.Users.First(y => y.Username == username),
                    _ => throw new InvalidOperationException()
                };
            }
            catch (InvalidOperationException)
            {
                return Redirect("/");
            }

            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            if (user.Admin) identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMonths(1),
                    IsPersistent = true,
                    AllowRefresh = false
                });
            
            return Redirect("/");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}