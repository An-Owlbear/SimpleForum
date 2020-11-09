﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;
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
    }
}