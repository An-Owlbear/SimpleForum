using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Web.Models;

namespace SimpleForum.Web.ViewComponents
{
    public class PostViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PostViewModel model)
        {
            return View("Post", model);
        }
    }
}