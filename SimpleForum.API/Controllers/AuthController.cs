using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Requests;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;
using SimpleForum.Common.Server;
using SimpleForum.Models;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthenticationManager _manager;
        private readonly SimpleForumRepository _repository;

        public AuthController(IAuthenticationManager manager, SimpleForumRepository repository)
        {
            _manager = manager;
            _repository = repository;
        }

        // Logs in the user
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            // Returns error if username or password are null
            if (loginRequest.Username == null || loginRequest.Password == null)
                return BadRequest("Username and password must not be null");

            // Attempts to create token, returns error if login details are incorrect
            Result<string> tokenResult = await _manager.Authenticate(loginRequest.Username, loginRequest.Password);
            if (tokenResult.Failure) return BadRequest(tokenResult.Error);

            // Creates and returns response
            LoginResponse response = new LoginResponse()
            {
                Token = tokenResult.Value,
                ValidUntil = DateTime.Now.AddMonths(1)
            };
            return Json(response);
        }

        [HttpPost("GenerateTempToken")]
        [Authorize]
        public async Task<IActionResult> GenerateTempToken()
        {
            User user = await _repository.GetUserAsync(User);
            TempApiToken token = await _repository.AddTempApiToken(user);
            await _repository.SaveChangesAsync();
            return Ok(token.Token);
        }
        
        // Tests authorisation is working
        [HttpGet("Test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok("Authorization successful");
        }
    }
}