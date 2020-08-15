using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NETCore.MailKit.Core;
using SimpleForum.Internal;
using SimpleForum.Models;
using SimpleForum.Web.Models;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SimpleForumConfig _config;
        private readonly IEmailService _emailService;
        
        string generateCode(int length)
        {
            string chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public LoginController(ApplicationDbContext context, IOptions<SimpleForumConfig> config, IEmailService emailService)
        {
            _context = context;
            _config = config.Value;
            _emailService = emailService;
        }
        
        [AnonymousOnly]
        public IActionResult Index(int? error, string ReturnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                Error = error,
                ReturnUrl = ReturnUrl
            };

            return View("Login", model);
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

            if (user.Banned)
            {
                MessageViewModel model = new MessageViewModel()
                {
                    Title = "Account banned",
                    MessageTitle = "Your account is banned",
                    MessageContent =  "Ban reason: " + user.BanReason
                };
                return View("Message", model);
            }

            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));

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

        [AnonymousOnly]
        public IActionResult ForgotPassword(int? error)
        {
            FormViewModel model = new FormViewModel()
            {
                Error = error
            };
            return View(model);
        }

        [AnonymousOnly]
        public async Task<IActionResult> SendForgotPassword(string email)
        {
            // Checks the entered email is valid
            if (email == null) return RedirectToAction("ForgotPassword", new {error = 0});
            
            User user;
            try
            {
                user = _context.Users.First(x => x.Email == email);
            }
            catch
            {
                return RedirectToAction("ForgotPassword", new {error = 1});
            }
            
            // Creates new EmailCode and adds it to database
            string code = generateCode(32);
            DateTime now = DateTime.Now;
            
            EmailCode emailCode = new EmailCode()
            {
                Code = code,
                DateCreated = DateTime.Now,
                Type = "password_reset",
                UserID = user.UserID,
                ValidUntil = now.AddHours(1)
            };
            await _context.EmailCodes.AddAsync(emailCode);
            await _context.SaveChangesAsync();

            // Sends password reset email
            string url = _config.InstanceURL + "/Login/ResetPassword?code=" + code;
            await _emailService.SendAsync(
                user.Email,
                "SimpleForum password reset",
                "<p>To reset your password, please click the following link: <a href=\"" + url +
                "\">" + url + "</a></p>",
                true
            );

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Email sent",
                MessageTitle = "Password reset email sent"
            };
            return View("Message", model);
        }

        [AnonymousOnly]
        public IActionResult ResetPassword(string code, int? error)
        {
            EmailCode emailCode;
            try
            {
                emailCode = _context.EmailCodes.First(x => x.Code == code);
            }
            catch
            {
                return Redirect("/");
            }
            
            ResetPasswordViewModel model = new ResetPasswordViewModel()
            {
                Error = error,
                Code = code,
                UserID = emailCode.UserID
            };
            
            return View(model);
        }

        public async Task<IActionResult> SendResetPassword(string password, string confirmPassword,
            string code, int? userID)
        {
            // Checks the parameters are valid and handles errors accordingly
            if (code == null || userID == null) return Redirect("/");
            if (password == null || confirmPassword == null) return RedirectToAction("ResetPassword", new {code, error = 0}); 
            if (password != confirmPassword) return RedirectToAction("ResetPassword", new {code, error = 1});

            // Checks the submitted userID and code are valid
            User user;
            EmailCode emailCode;
            try
            {
                user = _context.Users.First(x => x.UserID == userID);
                emailCode = _context.EmailCodes.First(x => x.Code == code);
            }
            catch
            {
                return Redirect("/");
            }

            if (user.UserID != emailCode.UserID) return Redirect("/");
            if (emailCode.ValidUntil < DateTime.Now) return Redirect("/");

            // Changed password and returns message
            user.Password = password;
            await _context.SaveChangesAsync();
            
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Password changed",
                MessageTitle = "Password changed successfully"
            };
            return View("Message", model);
        }
    }
}