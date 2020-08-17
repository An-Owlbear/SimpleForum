using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using SimpleForum.Web.Models;

namespace SimpleForum.Web
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IViewRenderService _viewRenderService;

        public CustomCookieAuthenticationEvents(IViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }

        public override async Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Forbidden",
                MessageTitle = "Access denied."
            };
            string response = await _viewRenderService.RenderToStringAsync("Message", model);
            await context.Response.WriteAsync(response);
        }
    }
}