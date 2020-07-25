using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using NETCore.MailKit.Core;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Controllers
{
    public class SignupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly SimpleForumConfig _config;

        public SignupController(ApplicationDbContext context, IEmailService emailService, IOptions<SimpleForumConfig> config)
        {
            _context = context;
            _emailService = emailService;
            _config = config.Value;
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
                Password = password
            };

            EntityEntry<User> userAdded = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Creates a random 32 character long string and adds to the email codes table
            // TODO - Verify the generated code does not already exist
            Random random = new Random();
            string chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
            string code = new string(Enumerable.Repeat(chars, 32).Select(s => s[random.Next(s.Length)]).ToArray());

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

            string url = _config.InstanceURL + "/Signup/VerifyEmail?code=" + code;
            await _emailService.SendAsync(
                email,
                "SimpleForum email confirmation",
                "<p>please confirm your email by clicking the following link: <a href='" + url +
                "'>" + url + "</a></p>",
                true);

            ViewData["MessageTitle"] = "Signup complete!";
            ViewData["MessageContent"] = "Before you can use your account your email must be verified. " +
                                        "We have sent a verification message to your email account";
            return View("Message");
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

            ViewData["MessageTitle"] = "Email verified sucessfully. You can now login.";
            return View("Message");
        }
    }
}