using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
            if (comment.Deleted || comment.User.Deleted || comment.UserPage.Deleted) return Gone("Comment deleted");

            return Json(new ApiComment(comment));
        }

        // Deletes a UserComment of the given ID
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            // Deletes the comment and returns error if unsuccessful
            Result result = await _repository.DeleteUserCommentAsync(id);
            if (result.Failure) return StatusCode(result.Code, result.Error);
            
            // Saves changes and return
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}