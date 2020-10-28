using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models;
using SimpleForum.Internal;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Threads")]
    public class ThreadsController : ControllerBase
    {
        private readonly SimpleForumRepository _repository;

        public ThreadsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet("")]
        public async Task<IEnumerable<Thread>> GetFrontPage(int page = 1)
        {
            IEnumerable<SimpleForum.Models.Thread> threads = await _repository.GetFrontPageAsync(page);
            return threads.Select(x => new Thread(x));
        }

        [HttpGet("{id}")]
        public async Task<Thread> GetThread(int id)
        {
            return new Thread(await _repository.GetThreadAsync(id));
        }
    }
}