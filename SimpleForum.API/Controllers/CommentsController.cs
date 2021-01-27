using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;
using SimpleForum.Common.Server;
using SimpleForum.Models;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Comments")]
    public class CommentsController : ApiController
    {
        private readonly SimpleForumRepository _repository;

        public CommentsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }

        // Gets a comment of the given ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            // Retrieves comment and returns null if not found
            Comment comment = await _repository.GetCommentAsync(id);
            if (comment == null) return NotFound("Comment not found");
            if (comment.Deleted || comment.Thread.Deleted || comment.User.Deleted) return Gone("Comment deleted");

            return Json(new ApiComment(comment));
        }

        // Deletes a comment of the given id
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            // Deletes comment and returns error if unsuccessful
            Result result = await _repository.DeleteCommentAsync(id);
            if (result.Failure) return StatusCode(result.Code, result.Error);
            
            // Saves changes and returns response
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}