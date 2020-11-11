using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleForum.API.Models.Responses;
using SimpleForum.Internal;

namespace SimpleForum.API.Policies
{
    public class PreventMuted : IAsyncActionFilter
    {
        private readonly SimpleForumRepository _repository;

        public PreventMuted(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Retrieves user and returns error if muted
            SimpleForum.Models.User user = await _repository.GetUserAsync(context.HttpContext.User);
            if (user.Muted)
            {
                context.Result = new JsonResult(new Error(403, "Account muted, access denied"));
                return;
            }

            await next();
        }
    }
}