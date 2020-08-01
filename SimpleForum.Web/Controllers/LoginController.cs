using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleForum.Internal;
using SimpleForum.Models;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SimpleForumConfig _config;

        public LoginController(ApplicationDbContext context, IOptions<SimpleForumConfig> config)
        {
            _context = context;
            _config = config.Value;
        }
        
        [AnonymousOnly]
        public IActionResult Index(int? error, string ReturnUrl)
        {
            List<string> errors = new List<string>()
            {
                "Enter both username and password",
                "Username or password is incorrect",
                "Email is not verified"
            };

            if (error != null) ViewData["error"] = errors[(int)error];
            ViewData["ReturnUrl"] = ReturnUrl;
            
            return View("Login");
        }

        [AnonymousOnly]
        public async Task<IActionResult> SendLogin(string username, string password, string ReturnUrl)
        {
            if (username == null || password == null) return Redirect("/Login?error=0");
            
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
                return Redirect("/Login?error=1");
            }

            if (user.Password != password) return Redirect("/Login?error=1");
            if (!user.Activated)
            {
                string resendUrl = _config.InstanceURL + "/Signup/ResendVerificationEmail?userID=" +
                                   user.UserID;
                ViewData["Title"] = ViewData["MessageTitle"] = "Email not verified";
                ViewData["MessageContent"] = "Before you can use your account your email must be verified. " +
                                             "We have sent a verification message to your email account.\n" +
                                             "If you have not received the email click <a class=top-message-link href='" +
                                             resendUrl + "'>here</a>.";
                return View("Message");
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
            
            return Redirect(ReturnUrl ?? "/");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}