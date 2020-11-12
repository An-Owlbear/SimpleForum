using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Requests;
using SimpleForum.API.Models.Responses;
using SimpleForum.API.Policies;
using SimpleForum.Internal;
using SimpleForum.Models;

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
        public async Task<IEnumerable<ApiThread>> GetFrontPage(int page = 1)
        {
            IEnumerable<Thread> threads = await _repository.GetFrontPageAsync(page);
            return threads.Select(x => new ApiThread(x));
        }

        // Returns a thread of the given ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetThread(int id)
        {
            Thread thread = await _repository.GetThreadAsync(id);
            if (thread == null) return NotFound("Requested thread not found");

            return Json(new ApiThread(thread));
        }

        // Gets a list of comments for a thread of the given ID
        [HttpGet("{id}/Comments")]
        public async Task<IEnumerable<ApiComment>> GetComments(int id, int page = 1)
        {
            IEnumerable<Comment> comments = await _repository.GetThreadRepliesAsync(id, page);
            return comments.Select(x => new ApiComment(x));
        }

        // Creates a new thread
        [HttpPut("")]
        [ServiceFilter(typeof(PreventMuted))]
        public async Task<IActionResult> CreateThread(CreateThreadRequest request)
        {
            // Returns error if either parameter is null
            if (request.Title == null || request.Content == null) return NotFound("Title and content cannot be null");

            // Creates and adds thread to database
            User user = await _repository.GetUserAsync(User);
            Thread thread = new Thread
            {
                Title = request.Title,
                Content = request.Content,
                DatePosted = DateTime.Now,
                User = user
            };
            await _repository.AddThreadAsync(thread);
            await _repository.SaveChangesAsync();

            // Returns JSON response
            return Json(new ApiThread(thread));
        }

        // Posts a comment to the thread
        [HttpPut("{id}/Comments")]
        [ServiceFilter(typeof(PreventMuted))]
        public async Task<IActionResult> PostComment(int id, PostCommentRequest request)
        {
            // Returns error if comment is null
            if (request.Content == null) return BadRequest("Comment cannot be empty");

            // Retrieves user and returns error if muted
            User user = await _repository.GetUserAsync(User);
            if (user.Muted) return Unauthorized("Your account is muted, you cannot post comments or create threads");

            // Retrieves thread and returns error if locked or not found / deleted
            Thread thread = await _repository.GetThreadAsync(id);
            if (thread == null || thread.Deleted) return NotFound("Thread not found");
            if (thread.Locked) return Forbid("Cannot reply, thread is locked");

            // Creates and adds comment to database
            Comment comment = new Comment()
            {
                Content = request.Content,
                DatePosted = DateTime.Now,
                ThreadID = id
            };
            await _repository.PostCommentAsync(comment);
            await _repository.SaveChangesAsync();

            // Returns JSON response
            return Json(new ApiComment(comment));
        }
    }
}