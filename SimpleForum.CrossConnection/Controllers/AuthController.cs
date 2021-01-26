using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Requests.CrossConnection;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.CrossConnection.Controllers
{
    [ApiController]
    [Route("/Auth")]
    public class AuthController : ApiController
    {
        private readonly SimpleForumRepository _repository;
        private readonly CrossConnectionClient _crossConnectionClient;

        public AuthController(SimpleForumRepository repository, CrossConnectionClient crossConnectionClient)
        {
            _repository = repository;
            _crossConnectionClient = crossConnectionClient;
        }
        
        [Authorize]
        [HttpGet("Test")]
        public string AuthTest()
        {
            return "Authentication successful";
        }

        // Checks an outgoing token exists, requested by the target server when registering an new token 
        [HttpGet("CheckToken")]
        public async Task<IActionResult> OutgoingTokenCheck(string token)
        {
            OutgoingServerToken outgoingToken = await _repository.GetOutgoingServerTokenByValueAsync(token);
            if (outgoingToken == null) return NotFound();
            return Ok();
        }

        // Adds an incoming server token to the database
        [HttpPut("RegisterToken")]
        public async Task<IActionResult> RegisterIncomingToken(IncomingServerToken request)
        {
            Result addResult = await _repository.AddIncomingServerToken(request);
            if (addResult.Failure) return StatusCode(addResult.Code, addResult.Error); 
            Result checkResult = await _crossConnectionClient.CheckToken(request.Address, request.Token);
            
            if (checkResult.Failure) return StatusCode(checkResult.Code, checkResult.Error);

            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}