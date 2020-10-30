using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;
using SimpleForum.Internal;

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
            SimpleForum.Models.Comment comment = await _repository.GetCommentAsync(id);
            if (comment == null) return NotFound("Comment not found");
            
            return Json(new Comment(comment));
        }
    }
}