using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Requests.CrossConnection;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;
using SimpleForum.Common.Server;
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

        // Checks if an incoming token for the given address has been registered
        [HttpGet("CheckAddress")]
        public async Task<IActionResult> CheckAddress(string address)
        {
            IncomingServerToken serverToken = await _repository.GetIncomingServerTokenByNameAsync(address);
            if (serverToken == null) return Ok();
            return Conflict();
        }

        // Adds an incoming server token to the database
        [HttpPut("RegisterToken")]
        public async Task<IActionResult> RegisterIncomingToken(RegisterTokenRequest request)
        {
            // Retrieves the urls of the services of the instance, and returns error if unsuccessful
            Result<ServerURLs> urlResult = await SimpleForumClient.GetServerURLs(request.Address);
            if (urlResult.Failure) return StatusCode(urlResult.Code, urlResult.Error);
            
            // Adds the token to checks it is valid
            IncomingServerToken token = new IncomingServerToken()
            {
                Address = request.Address,
                ApiAddress = urlResult.Value.APIURL,
                CrossConnectionAddress = urlResult.Value.CrossConnectionURL,
                Token = request.Token
            };
            
            Result addResult = await _repository.AddIncomingServerToken(token);
            if (addResult.Failure) return StatusCode(addResult.Code, addResult.Error); 
            Result checkResult = await _crossConnectionClient.CheckToken(token.CrossConnectionAddress, request.Token);
            
            // Returns error if token is invalid, otherwise saves changes
            if (checkResult.Failure) return StatusCode(checkResult.Code, checkResult.Error);
            await _repository.SaveChangesAsync();
            return Ok();
        }
    }
}