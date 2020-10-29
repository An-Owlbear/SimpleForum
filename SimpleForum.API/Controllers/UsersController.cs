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
        private readonly IAuthenticationManager _manager;

        public UserController(SimpleForumRepository repository, IAuthenticationManager manager)
        {
            _repository = repository;
            _manager = manager;
        }
        
        // Gets a user of the given ID
        [HttpGet("{id}")]
        public async Task<User> GetUser(int id)
        {
            return new User(await _repository.GetUserAsync(id));
        }

        // Gets a list of UserComments for a user of the given ID
        [HttpGet("{id}/Comments")]
        public async Task<IEnumerable<Comment>> GetUserComments(int id, int page = 1)
        {
            IEnumerable<SimpleForum.Models.UserComment> comments = await _repository.GetUserCommentsAsync(id, page);
            return comments.Select(x => new Comment(x));
        }
    }
}