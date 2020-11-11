using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Requests;
using SimpleForum.API.Models.Responses;
using SimpleForum.API.Policies;
using SimpleForum.Internal;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Users")]
    public class UserController : ApiController
    {
        private readonly SimpleForumRepository _repository;

        public UserController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        // Gets a user of the given ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            SimpleForum.Models.User user = await _repository.GetUserAsync(id);
            if (user == null) return NotFound("User not found");

            return Json(new User(user));
        }

        // Gets a list of UserComments for a user of the given ID
        [HttpGet("{id}/Comments")]
        public async Task<IEnumerable<Comment>> GetUserComments(int id, int page = 1)
        {
            IEnumerable<SimpleForum.Models.UserComment> comments = await _repository.GetUserCommentsAsync(id, page);
            return comments.Select(x => new Comment(x));
        }
        
        // Posts a comment to a user's profile
        [HttpPut("{id}/Comments")]
        [ServiceFilter(typeof(PreventMuted))]
        public async Task<IActionResult> PostComment(int id, PostCommentRequest request)
        {
            // Returns error if comment is empty
            if (request.Content == null) return BadRequest("Comment cannot be null");

            // Retrieves user and return not found if thread not found
            SimpleForum.Models.User currentUser = await _repository.GetUserAsync(User);
            SimpleForum.Models.User profileUser = await _repository.GetUserAsync(id);
            if (profileUser == null || profileUser.Deleted) return NotFound("Requested user not found");
            
            // Creates user comment and adds it to database
            SimpleForum.Models.UserComment userComment = new SimpleForum.Models.UserComment()
            {
                Content = request.Content,
                DatePosted = DateTime.Now,
                User = currentUser,
                UserPage = profileUser
            };
            await _repository.PostUserCommentAsync(userComment);
            await _repository.SaveChangesAsync();
            
            // Returns JSON response
            return Json(new Comment(userComment));
        }
    }
}