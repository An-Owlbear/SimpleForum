using Microsoft.AspNetCore.Mvc;
using SimpleForum.Web.Models;

namespace SimpleForum.Web.ViewComponents
{
    public class PostViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PostViewModel model)
        {
            return View("Post", model);
        }
    }
}