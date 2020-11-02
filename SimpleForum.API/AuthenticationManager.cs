using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.API
{
    public interface IAuthenticationManager
    {
        Task<string> Authenticate(string username, string password);
    }
    
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly SimpleForumRepository _repository;
        private readonly SimpleForumConfig _config;
        
        public AuthenticationManager(SimpleForumRepository repository, IOptions<SimpleForumConfig> config)
        {
            _repository = repository;
            _config = config.Value;
        }
        
        // Authenticates a user
        public async Task<string> Authenticate(string username, string password)
        {
            // Gets a user and throws exception if username or password is incorrect
            User user;
            try
            {
                user = await _repository.GetUserAsync(username);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("username incorrect");
            }
            if (user.Password != password) throw new InvalidOperationException("password incorrect");
            if (user.Deleted) throw new InvalidOperationException("username incorrect");
            
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
            return tokenHandler.WriteToken(token);
        }
    }
}