using Microsoft.AspNetCore.Mvc;
using SimpleForum.Web.Models;

namespace SimpleForum.Web
{
    public class WebController : Controller
    {
        public IActionResult StatusCode(int code, string message)
        {
            MessageViewModel model = new MessageViewModel()
            {
                MessageTitle = $"Error {code} - {message}"
            };

            ViewResult result = View("Message", model);
            result.StatusCode = code;
            return result;
        }
    }
}