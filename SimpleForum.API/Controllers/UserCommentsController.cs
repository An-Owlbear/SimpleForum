using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;
using SimpleForum.Internal;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("UserComments")]
    public class UserCommentsController : Controller
    {
        private readonly SimpleForumRepository _repository;

        public UserCommentsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }

        // Gets a UserComment of the given ID
        [HttpGet("{id}")]
        public async Task<Comment> GetUserComment(int id)
        {
            return new Comment(await _repository.GetUserCommentAsync(id));
        }
    }
}