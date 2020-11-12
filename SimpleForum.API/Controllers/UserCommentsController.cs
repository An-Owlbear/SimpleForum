using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("UserComments")]
    public class UserCommentsController : ApiController
    {
        private readonly SimpleForumRepository _repository;

        public UserCommentsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }

        // Gets a UserComment of the given ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserComment(int id)
        {
            // Retrieves comment and returns 404 if null
            UserComment comment = await _repository.GetUserCommentAsync(id);
            if (comment == null) return NotFound("UserComment not found");

            return Json(new ApiComment(comment));
        }
    }
}