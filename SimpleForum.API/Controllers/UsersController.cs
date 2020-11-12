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
            User user = await _repository.GetUserAsync(id);
            if (user == null) return NotFound("User not found");

            return Json(new ApiUser(user));
        }

        // Gets a list of UserComments for a user of the given ID
        [HttpGet("{id}/Comments")]
        public async Task<IEnumerable<ApiComment>> GetUserComments(int id, int page = 1)
        {
            IEnumerable<UserComment> comments = await _repository.GetUserCommentsAsync(id, page);
            return comments.Select(x => new ApiComment(x));
        }

        // Posts a comment to a user's profile
        [HttpPut("{id}/Comments")]
        [ServiceFilter(typeof(PreventMuted))]
        public async Task<IActionResult> PostComment(int id, PostCommentRequest request)
        {
            // Returns error if comment is empty
            if (request.Content == null) return BadRequest("Comment cannot be null");

            // Retrieves user and returns error if user is not found, deleted or locked
            User currentUser = await _repository.GetUserAsync(User);
            User profileUser = await _repository.GetUserAsync(id);
            if (profileUser == null || profileUser.Deleted) return NotFound("Requested user not found");
            if (profileUser.CommentsLocked) return Forbid("The user's comments are locked");

            // Creates user comment and adds it to database
            UserComment userComment = new UserComment()
            {
                Content = request.Content,
                DatePosted = DateTime.Now,
                User = currentUser,
                UserPage = profileUser
            };
            await _repository.PostUserCommentAsync(userComment);
            await _repository.SaveChangesAsync();

            // Returns JSON response
            return Json(new ApiComment(userComment));
        }
    }
}