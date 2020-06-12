using Microsoft.AspNetCore.Mvc;

namespace SimpleForum.Web.Controllers
{
    public class LoginController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View("Login");
        }
    }
}