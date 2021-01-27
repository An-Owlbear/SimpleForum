using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] tokenKey = Encoding.ASCII.GetBytes(_config.PrivateKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
                }),
                Expires = DateTime.Now.AddMonths(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return Result.Ok(tokenHandler.WriteToken(token));
        }
    }
}