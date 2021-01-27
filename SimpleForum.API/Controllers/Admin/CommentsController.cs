using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Requests;
using SimpleForum.Common;
using SimpleForum.Common.Server;

namespace SimpleForum.API.Controllers.Admin
{
    [ApiController]
    [Route("Admin/Comments")]
    public class CommentsController : ApiController
    {
        private readonly SimpleForumRepository _repository;

        public CommentsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        // Deletes a comment as admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDeleteComment(int id, AdminDeleteRequest request)
        {
            // Returns error if reason is null
            if (request.Reason == null) return BadRequest("Reason cannot be null");
            
            // Deletes the comment and returns 403 if unauthorised
            Result result = await _repository.AdminDeleteCommentAsync(id, request.Reason);
            if (result.Failure) return StatusCode(result.Code, result.Error);
            
            // Saves changes and returns
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}