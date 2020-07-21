using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NETCore.MailKit.Core;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Controllers
{
    public class SignupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public SignupController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

            await _emailService.SendAsync(
                email,
                "SimpleForum email confirmation",
                "<p>please confirm your email by clicking the following link: <a href=\'https://example.com/Signup/VerifyEmail?code=" + code +
                "'>https://example.com/Signup/VerifyEmail?code=" + code + "</a></p>",
                true);

            // TODO - Add to tell user about verification email
            return Redirect("/Login");
        }

        public async Task<IActionResult> VerifyEmail(string code)
        {
            // TODO - Create email verified view

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

            return Redirect("/");
        }
    }
}