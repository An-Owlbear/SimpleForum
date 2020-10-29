using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models;
using SimpleForum.Internal;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Users")]
    public class UserController : ControllerBase
    {
        private readonly SimpleForumRepository _repository;

        public UserController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet("{id}")]
        public async Task<User> GetUser(int id)
        {
            return new User(await _repository.GetUserAsync(id));
        }

        [HttpGet("{id}/Comments")]
        public async Task<IEnumerable<Comment>> GetUserComment(int id, int page = 1)
        {
            IEnumerable<SimpleForum.Models.UserComment> comments = await _repository.GetUserCommentsAsync(id, page);
            return comments.Select(x => new Comment(x));
        }
    }
}