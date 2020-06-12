using Microsoft.AspNetCore.Mvc;

namespace SimpleForum.Web.Controllers
{
    public class SignupController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View("Signup");
        }
    }
}