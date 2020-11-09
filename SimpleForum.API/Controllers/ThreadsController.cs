﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;
using SimpleForum.Internal;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Threads")]
    public class ThreadsController : ApiController
    {
        private readonly SimpleForumRepository _repository;

        public ThreadsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        // Returns a list of threads for the given page
        [HttpGet("")]
        public async Task<IEnumerable<Thread>> GetFrontPage(int page = 1)
        {
            IEnumerable<SimpleForum.Models.Thread> threads = await _repository.GetFrontPageAsync(page);
            return threads.Select(x => new Thread(x));
        }

        // Returns a thread of the given ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetThread(int id)
        {
             SimpleForum.Models.Thread thread = await _repository.GetThreadAsync(id);
             if (thread == null) return NotFound("Requested thread not found");

             return Json(new Thread(thread));
        }

        // Gets a list of comments for a thread of the given ID
        [HttpGet("{id}/Comments")]
        public async Task<IEnumerable<Comment>> GetComments(int id, int page = 1)
        {
            IEnumerable<SimpleForum.Models.Comment> comments = await _repository.GetThreadRepliesAsync(id, page);
            return comments.Select(x => new Comment(x));
        }
    }
}