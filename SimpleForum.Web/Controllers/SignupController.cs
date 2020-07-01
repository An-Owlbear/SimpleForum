using System;
using System.Collections.Generic;
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
    public class SignupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SignupController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IActionResult Index(int? error)
        {
            List<string> errors = new List<string>()
            {
                "Enter email, username and password",
                "Email not available",
                "Username not available"
            };

            if (error != null) ViewData["error"] = errors[(int)error];
            
            return View("Signup");
        }

        public async Task<IActionResult> SendSignup(string email, string username, string password)
        {
            if (User.Identity.IsAuthenticated) return Redirect("/");
            if (email == null || username == null || password == null) return Redirect("/Signup?error=0");
            if (_context.Users.Any(x => x.Email == email)) return Redirect("/Signup?error=1");
            if (_context.Users.Any(x => x.Username == username)) return Redirect("/Signup?error=2");

            User user = new User()
            {
                Email = email,
                Username = username,
                Password = password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
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
    }
}