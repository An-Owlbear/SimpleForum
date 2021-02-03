using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SimpleForum.Common;
using SimpleForum.Common.Server;
using SimpleForum.Models;

namespace SimpleForum.API
{
    public interface IAuthenticationManager
    {
        Task<Result<string>> Authenticate(string username, string password);
    }
    
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly SimpleForumRepository _repository;
        private readonly SimpleForumConfig _config;
        
        public AuthenticationManager(SimpleForumRepository repository, IOptionsSnapshot<SimpleForumConfig> config)
        {
            _repository = repository;
            _config = config.Value;
        }
        
        // Authenticates a user
        public async Task<Result<string>> Authenticate(string username, string password)
        {
            // Gets a user and returns failure if details are incorrect
            User user = await _repository.GetUserAsync(username);
            if (user == null)
            {
                return Result.Fail<string>("username incorrect", 400);
            }

            if (user.Password != password) return Result.Fail<string>("password incorrect", 400);
            if (user.Deleted) return Result.Fail<string>("username incorrect", 400);
            
            // Creates and returns a JWT token
            string token = JwtToken.CreateToken(username, user.UserID.ToString(), _config.PrivateKey);
            return Result.Ok(token);
        }
    }
}