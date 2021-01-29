using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.API.Models.Responses.CrossConnection;
using SimpleForum.Models;

namespace SimpleForum.Common.Server
{
    /// <summary>
    /// Methods for interacting across instances
    /// </summary>
    /// <remarks>
    /// Unlike SimpleForumRepository, these methods will save changes automatically,
    /// as it is required for some operations
    /// </remarks>
    public class CrossConnectionManager
    {
        private readonly SimpleForumRepository _repository;
        private readonly CrossConnectionClient _client;
        private readonly SimpleForumConfig _config;

        public CrossConnectionManager(SimpleForumRepository repository, CrossConnectionClient client, 
            IOptionsSnapshot<SimpleForumConfig> config)
        {
            _repository = repository;
            _client = client;
            _config = config.Value;
        }

        // Registers an outgoing token on the given server
        public async Task<Result> RegisterToken(string address)
        {
            // Retrieves ServerURLs
            Result<ServerURLs> result = await SimpleForumClient.GetServerURLs(address);
            if (result.Failure) return result;
            ServerURLs serverUrLs = result.Value;
            
            // Checks if the address has already been registered
            Result checkResult = await _client.CheckAddress(serverUrLs.CrossConnectionURL, _config.InstanceURL);
            if (checkResult.Failure) return checkResult;
            
            
            // Adds token to repository and 
            OutgoingServerToken token = new OutgoingServerToken()
            {
                Address = address,
                ApiAddress = serverUrLs.APIURL,
                CrossConnectionAddress = serverUrLs.CrossConnectionURL,
                Token = Guid.NewGuid().ToString()
            };

            await _repository.AddOutgoingServerTokenAsync(token);
            await _repository.SaveChangesAsync();
            Result registerResult = await _client.RegisterAddress(serverUrLs.CrossConnectionURL, _config.InstanceURL, token.Token);
            if (registerResult.Failure) return registerResult;
            return Result.Ok();
        }

        // Ensures that the server being contacted has a registered token
        public async Task SetupContact(string address)
        {
            OutgoingServerToken checkResult = await _repository.GetOutgoingServerTokenByNameAsync(address);
            if (checkResult == null) await RegisterToken(address);
        }

        // Retrieves the token and adds the user to the database
        public async Task<Result<User>> AuthenticateUser(string address, string token)
        {
            // Finds server ID, and returns failure if none found
            IncomingServerToken server = await _repository.GetIncomingServerTokenByNameAsync(address);
            if (server == null) return Result.Fail<User>("Invalid address", 400);
            
            // Checks tokens, and returns failure if invalid
            Result<CrossConnectionUser> result = await _client.AuthenticateToken(server.CrossConnectionAddress, token);
            if (result.Failure) return Result.Fail<User>(result.Error, result.Code);
            
            // Finds user and returns if no null, otherwise creates new user
            User user = await _repository.GetRemoteUserAsync(server.IncomingServerTokenID, result.Value.Username);
            if (user != null) return Result.Ok(user);

            // Creates new user and adds to database
            User newUser = new User()
            {
                Username = result.Value.Username,
                Email = result.Value.Email,
                ServerID = server.IncomingServerTokenID,
                SignupDate = DateTime.Now
            };
            Result signUpResult = await _repository.SignupAsync(newUser);
            await _repository.SaveChangesAsync();
            if (signUpResult.Failure) return Result.Fail<User>(signUpResult.Error, signUpResult.Code);
            return Result.Ok(newUser);
        }
    }
}