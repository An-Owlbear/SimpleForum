using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
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
    }
}