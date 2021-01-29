using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Requests.CrossConnection;
using SimpleForum.API.Models.Responses.CrossConnection;
using SimpleForum.Common.Server;
using SimpleForum.Models;

namespace SimpleForum.CrossConnection.Controllers
{
    [ApiController]
    [Route("/User")]
    public class UserController : ApiController
    {
        private readonly SimpleForumRepository _repository;

        public UserController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        // Authenticates a user for the given auth token
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateUserRequest request)
        {
            RemoteAuthToken authToken = await _repository.GetRemoteAuthTokenAsync(request.Token);
            if (authToken == null || authToken.ValidUntil < DateTime.Now) return NotFound();
            return Json(new CrossConnectionUser(authToken.User));
        }
    }
}