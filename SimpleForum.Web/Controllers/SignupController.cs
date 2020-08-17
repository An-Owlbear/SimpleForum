using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly SimpleForumConfig _config;

        string generateCode(int length)
        {
            string chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public SignupController(ApplicationDbContext context, IEmailService emailService,
            IOptions<SimpleForumConfig> config)
        {
            _context = context;
            _emailService = emailService;
            _config = config.Value;
        }

        [AnonymousOnly]
        public IActionResult Index(int? error)
        {
            FormViewModel model = new FormViewModel()
            {
                Error = error
            };

            return View("Signup", model);
        }

        [AnonymousOnly]
        public async Task<IActionResult> SendSignup(string email, string username, string password)
        {
            // Validates the user input and redirects them if necessary
            if (User.Identity.IsAuthenticated) return Redirect("/");
            if (email == null || username == null || password == null) return Redirect("/Signup?error=0");
            if (_context.Users.Any(x => x.Email == email)) return Redirect("/Signup?error=1");
            if (_context.Users.Any(x => x.Username == username)) return Redirect("/Signup?error=2");

            // Creates a user object from user input and adds it to the database
            User user = new User()
            {
                Email = email,
                Username = username,
                Password = password,
                Activated = false,
                SignupDate = DateTime.Now
            };

            EntityEntry<User> userAdded = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Creates a random 32 character long string and adds to the email codes table
            // TODO - Verify the generated code does not already exist
            string code = generateCode(32);

            EmailCode emailCode = new EmailCode()
            {
                Code = code,
                Type = "signup",
                DateCreated = DateTime.Now,
                ValidUntil = DateTime.Now.AddHours(24),
                UserID = userAdded.Entity.UserID
            };

            await _context.EmailCodes.AddAsync(emailCode);
            await _context.SaveChangesAsync();

            // Sends an email containing the link to verify the email
            string url = _config.InstanceURL + "/Signup/VerifyEmail?code=" + code;
            await _emailService.SendAsync(
                email,
                "SimpleForum email confirmation",
                "<p>please confirm your email by clicking the following link: <a href='" + url +
                "'>" + url + "</a></p>",
                true);

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
                               userAdded.Entity.UserID;
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

        public async Task<IActionResult> VerifyEmail(string code)
        {
            EmailCode emailCode;
            try
            {
                emailCode = _context.EmailCodes.First(x => x.Code == code);
            }
            catch (InvalidOperationException)
            {
                return Redirect("/");
            }

            if (emailCode.ValidUntil < DateTime.Now) return Redirect("/");

            emailCode.User.Activated = true;
            await _context.SaveChangesAsync();
            
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Email verified",
                MessageTitle = "Email verified successfully, you can now access all features"
            };
            return View("Message", model);
        }

        public async Task<IActionResult> ResendVerificationEmail(int? userID)
        {
            EmailCode emailCode;
            try
            {
                emailCode = _context.EmailCodes.First(x => x.UserID == userID);
            }
            catch (InvalidOperationException)
            {
                return Redirect("/");
            }

            string code = generateCode(32);

            EmailCode newEmailCode = new EmailCode()
            {
                Code = code,
                Type = "Signup",
                DateCreated = DateTime.Now,
                ValidUntil = DateTime.Now.AddHours(24),
                UserID = emailCode.UserID
            };

            await _context.EmailCodes.AddAsync(newEmailCode);
            await _context.SaveChangesAsync();
            
            string url = _config.InstanceURL + "/Signup/VerifyEmail?code=" + code;
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