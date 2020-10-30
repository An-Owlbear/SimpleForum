﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;
using SimpleForum.Internal;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Comments")]
    public class CommentsController : ApiController
    {
        private readonly SimpleForumRepository _repository;

        public CommentsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        // Gets a comment of the given ID
        [HttpGet("{id}")]
        public async Task<Comment> GetComment(int id)
        {
            return new Comment(await _repository.GetCommentAsync(id));
        }
    }
}