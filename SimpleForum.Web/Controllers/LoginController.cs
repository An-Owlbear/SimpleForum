using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Internal;
using SimpleForum.Models;
using SimpleForum.Web.Models;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly SimpleForumRepository _repository;

        public LoginController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        // Returns the login page
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

        // Logs in a user
        [AnonymousOnly]
        public async Task<IActionResult> SendLogin(string username, string password, string ReturnUrl)
        {
            // Returns error if username or password are null
            if (username == null || password == null) return Redirect("/Login?error=0");
            
            // Retrieves user, returning error if none are found
            User user;
            try
            {
                user = await _repository.GetUserAsync(username);
            }
            catch (InvalidOperationException)
            {
                return Redirect("/Login?error=1");
            }

            // Returns error if password is incorrect
            if (user.Password != password || user.Deleted) return Redirect("/Login?error=1");

            // Returns error if user is banned
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

            // Creates a ClaimIdentity for the user
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            
            // Signs in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMonths(1),
                    IsPersistent = true,
                    AllowRefresh = false
                });

            // Redirects the user to the previously requested page if needed, otherwise returns to index
            if (Url.IsLocalUrl(ReturnUrl)) return Redirect(ReturnUrl);
            return Redirect("/");
        }

        // Signs out a user
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        // Returns the page for submitting a password reset request
        [AnonymousOnly]
        public IActionResult ForgotPassword(int? error)
        {
            FormViewModel model = new FormViewModel()
            {
                Error = error
            };
            return View(model);
        }

        // Requests a password reset
        [AnonymousOnly]
        public async Task<IActionResult> SendForgotPassword(string email)
        {
            // Checks the entered email is valid
            if (email == null) return RedirectToAction("ForgotPassword", new {error = 0});
            
            // Retrieves user and returns error if none found
            User user = await _repository.GetUserAsync(email);
            if (user == null) return RedirectToAction("ForgotPassword", new {error = 1});

            // Requests the password reset
            await _repository.RequestPasswordResetAsync(user);
            await _repository.SaveChangesAsync();
            
            // Returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Email sent",
                MessageTitle = "Password reset email sent"
            };
            return View("Message", model);
        }

        // The page for submitting password resets
        [AnonymousOnly]
        public async Task<IActionResult> ResetPassword(string code, int? error)
        {
            // Retrieves email code and redirects if null
            EmailCode emailCode = await _repository.GetEmailCodeAsync(code);
            if (emailCode == null) return Redirect("/");

            ResetPasswordViewModel model = new ResetPasswordViewModel()
            {
                Error = error,
                Code = code,
                UserID = emailCode.UserID
            };
            
            return View(model);
        }
        
        // Resets a user's password
        [AnonymousOnly]
        public async Task<IActionResult> SendResetPassword(string password, string confirmPassword,
            string code, int userID)
        {
            // Checks the parameters are valid and handles errors accordingly
            if (code == null) return Redirect("/");
            if (password == null || confirmPassword == null) return RedirectToAction("ResetPassword", new {code, error = 0}); 
            if (password != confirmPassword) return RedirectToAction("ResetPassword", new {code, error = 1});

            // Changes the password, returning an error if permission is denied
            Result result = await _repository.ResetPasswordAsync(password, code, userID);
            if (result.Failure) return new StatusCodeResult(result.Code);
            await _repository.SaveChangesAsync();
            

            // Returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Password changed",
                MessageTitle = "Password changed successfully"
            };
            return View("Message", model);
        }
    }
}