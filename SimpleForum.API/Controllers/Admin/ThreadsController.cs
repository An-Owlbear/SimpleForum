using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Requests;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;
using SimpleForum.Common.Server;
using SimpleForum.Models;

namespace SimpleForum.API.Controllers.Admin
{
    [ApiController]
    [Route("Admin/Threads")]
    public class ThreadsController : ApiController
    {
        private readonly SimpleForumRepository _repository;
        
        public ThreadsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        // Updates the information of the thread as an admin
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminUpdate(int id, UpdateThreadRequest request)
        {
            // Retrieves thread and updates information
            Thread thread = await _repository.GetThreadAsync(id);
            thread.Pinned = request.Pinned ?? thread.Pinned;
            thread.Locked = request.Locked ?? thread.Locked;
            await _repository.SaveChangesAsync();

            // Returns response
            return Json(new ApiThread(thread));
        }
        
        // Deletes thread as an admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDeleteThread(int id, AdminDeleteRequest request)
        {
            // Returns error if reason is null
            if (request.Reason == null) return BadRequest("Reason cannot be null");
            
            // Deletes thread and returns 403 if unauthorized
            Result result = await _repository.AdminDeleteThreadAsync(id, request.Reason);
            if (result.Failure) return StatusCode(result.Code, result.Error);
            
            // Saves changes and returns
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}