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
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            // Returns error if username or password are null
            if (loginRequest.Username == null || loginRequest.Password == null)
                return BadRequest("Username and password must not be null");
            
            // Attempts to create token, returns error if login details are incorrect
            string token;
            try
            {
                token = await _manager.Authenticate(loginRequest.Username, loginRequest.Password);
            }
            catch (InvalidOperationException e) when (e.Message == "username incorrect")
            {
                return BadRequest("Username is incorrect");
            }
            catch (InvalidOperationException e) when (e.Message == "password incorrect")
            {
                return BadRequest("Password is incorrect");
            }

            // Creates and returns response
            LoginResponse response = new LoginResponse()
            {
                Token = token,
                ValidUntil = DateTime.Now.AddMonths(1)
            };
            return Json(response);
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