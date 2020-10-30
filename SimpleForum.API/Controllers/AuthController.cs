using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Requests;
using SimpleForum.API.Models.Responses;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthenticationManager _manager;

        public AuthController(IAuthenticationManager manager)
        {
            _manager = manager;
        } 

        // Logs in the user
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            string token = await _manager.Authenticate(loginRequest.Username, loginRequest.Password);
            if (token == null) return Unauthorized();
            LoginResponse response = new LoginResponse()
            {
                Token = token,
                ValidUntil = DateTime.Now.AddMonths(1)
            };

            return Json(response);
        }

        [HttpGet("Test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok("Authorization successful");
        }
    }
}