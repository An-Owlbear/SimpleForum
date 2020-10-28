using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models;
using SimpleForum.Internal;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Comments")]
    public class CommentsController : ControllerBase
    {
        private readonly SimpleForumRepository _repository;

        public CommentsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet("{id}")]
        public async Task<Comment> GetComment(int id)
        {
            return new Comment(await _repository.GetCommentAsync(id));
        }
    }
}