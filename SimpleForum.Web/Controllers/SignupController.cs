using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleForum.Common;
using SimpleForum.Common.Server;
using SimpleForum.Models;
using SimpleForum.Web.Models;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web.Controllers
{
    public class SignupController : WebController
    {
        private readonly SimpleForumRepository _repository;
        private readonly SimpleForumConfig _config;

        public SignupController(SimpleForumRepository repository, IOptionsSnapshot<SimpleForumConfig> config)
        {
            _repository = repository;
            _config = config.Value;
        }

        // Returns the main signup view
        [AnonymousOnly]
        public IActionResult Index(int? error)
        {
            FormViewModel model = new FormViewModel()
            {
                Error = error
            };
            return View("Signup", model);
        }

        // Creates a user account
        [AnonymousOnly]
        public async Task<IActionResult> SendSignup(string email, string username, string password, string confirmPassword)
        {
            // Returns if any inputs are null
            if (email == null || username == null || password == null) return RedirectToAction("Index", new {error = 0});
            // Returns if passwords do not match
            if (password != confirmPassword) return RedirectToAction("Index", new {error = 3});
            
            // Creates and adds new user
            User user = new User()
            {
                Username = username,
                Email = email,
                Password = password,
                Activated = true
            };
            User addedUser = await _repository.AddUserAsync(user);
            await _repository.SaveChangesAsync();
            
            // Creates ClaimsPrincipal
            ClaimsPrincipal principal = Auth.CreateClaims(user);

            // Signs in user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMonths(1),
                    IsPersistent = true,
                    AllowRefresh = false
                });

            // Returns a signup complete page informing the user about email verification, containing a button to
            // resend the verification email.
            string resendUrl = _config.InstanceURL + "/Signup/ResendVerificationEmail?userID=" +
                               addedUser.UserID;
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Signup complete",
                MessageTitle = "Signup complete",
                MessageContent = "Some features may be restricted until your email is verified. " +
                                 "We have sent a verification message to your email account.\n" +
                                 $"If you have not received the email click [here]({resendUrl})."
            };

            return View("Message", model);
        }

        // Verifies a user's email
        public async Task<IActionResult> VerifyEmail(string code)
        {
            // Retrieves code and redirects if it has expired
            EmailCode emailCode = await _repository.GetEmailCodeAsync(code);
            if (emailCode.ValidUntil < DateTime.Now) return Redirect("/");

            // Sets the user as activated and sets the code as invalid
            emailCode.Valid = false;
            emailCode.User.Activated = true;
            await _repository.SaveChangesAsync();
            
            // Creates the model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Email verified",
                MessageTitle = "Email verified successfully, you can now access all features"
            };
            return View("Message", model);
        }

        // Resends the verification email
        public async Task<IActionResult> ResendVerificationEmail(int userID)
        {
            //  Retrieves the user to resend the code for
            User user = await _repository.GetUserAsync(User);
            
            // Creates new emailCode and saves changes
            await _repository.ResendSignupCode(user);
            await _repository.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Email resent",
                MessageTitle = "Verification email resent"
            };
            return View("Message", model);
        }
    }
}