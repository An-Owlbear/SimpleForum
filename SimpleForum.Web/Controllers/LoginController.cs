using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Internal;

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

        public IActionResult SendLogin(string username, string password)
        {
            if (username == null || password == null) return Redirect("/");

            return View("Login");
        }
    }
}