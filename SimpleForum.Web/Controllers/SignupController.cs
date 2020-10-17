using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NETCore.MailKit.Core;
using SimpleForum.Internal;
using SimpleForum.Models;
using SimpleForum.Web.Models;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web.Controllers
{
    public class SignupController : Controller
    {
        private readonly SimpleForumRepository _repository;
        private readonly IEmailService _emailService;
        private readonly SimpleForumConfig _config;

        public SignupController(SimpleForumRepository repository, IEmailService emailService, IOptions<SimpleForumConfig> config)
        {
            _repository = repository;
            _emailService = emailService;
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
        public async Task<IActionResult> SendSignup(string email, string username, string password)
        {
            // Creates and adds new user
            User user = new User()
            {
                Username = username,
                Email = email,
                Password = password
            };
            (User addedUser, EmailCode emailCode) = await _repository.SignupAsync(user);
            await _repository.SaveChangesAsync();

            // Sends an email containing the link to verify the email
            string url = _config.InstanceURL + "/Signup/VerifyEmail?code=" + emailCode;

            await _emailService.SendAsync(
                email,
                "SimpleForum email confirmation",
                "<p>please confirm your email by clicking the following link: <a href='" + url +
                "'>" + url + "</a></p>",
                true
            );

            // Logs in the user
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name, ClaimTypes.Role);
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
            EmailCode emailCode = await _repository.ResendSignupCode(user);
            await _repository.SaveChangesAsync();
            
            string url = _config.InstanceURL + "/Signup/VerifyEmail?code=" + emailCode.Code;
            await _emailService.SendAsync(
                emailCode.User.Email,
                "SimpleForum email confirmation",
                "<p>please confirm your email by clicking the following link: <a href='" + url +
                "'>" + url + "</a></p>",
                true);

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Email resent",
                MessageTitle = "Verification email resent"
            };
            return View("Message", model);
        }
    }
}