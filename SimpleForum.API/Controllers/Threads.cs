using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models;
using SimpleForum.Internal;

namespace SimpleForum.API.Controllers
{
    public class ThreadsController : ControllerBase
    {
        private readonly SimpleForumRepository _repository;

        public ThreadsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        [Route("Threads/")]
        [HttpGet]
        public async Task<IEnumerable<Thread>> Index(int page = 1)
        {
            IEnumerable<SimpleForum.Models.Thread> threads = await _repository.GetFrontPageAsync(page);
            return threads.Select(x => new Thread(x));
        }
    }
}