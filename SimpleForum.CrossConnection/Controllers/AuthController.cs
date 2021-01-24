using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Requests;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.CrossConnection.Controllers
{
    [ApiController]
    [Route("/Auth")]
    public class AuthController : ApiController
    {
        private readonly SimpleForumRepository _repository;

        public AuthController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        [Authorize]
        [HttpGet("Test")]
        public string AuthTest()
        {
            return "Authentication successful";
        }

        // Checks an outgoing token exists, requested by the target server when registering an new token 
        [HttpGet("CheckToken")]
        public async Task<IActionResult> OutgoingTokenCheck(CheckTokenRequest request)
        {
            OutgoingServerToken token = await _repository.GetOutgoingServerTokenAsync(request.Token);
            if (token == null) return NotFound();
            return Ok();
        }
    }
}