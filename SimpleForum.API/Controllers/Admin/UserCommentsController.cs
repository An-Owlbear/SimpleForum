using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Requests;
using SimpleForum.Common;
using SimpleForum.Common.Server;

namespace SimpleForum.API.Controllers.Admin
{
    [ApiController]
    [Route("Admin/UserComments")]
    public class UserCommentsController : ApiController
    {
        private readonly SimpleForumRepository _repository;

        public UserCommentsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        // Deletes a user comments as admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDeleteUserComment(int id, AdminDeleteRequest request)
        {
            // Returns error ir reason is null
            if (request.Reason == null) return BadRequest("Reason cannot be null");
            
            // Deletes comment and returns error if unauthorised
            Result result = await _repository.AdminDeleteUserCommentAsync(id, request.Reason);
            if (result.Failure) return StatusCode(result.Code, result.Error);
            
            // Saves changes and returns
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}