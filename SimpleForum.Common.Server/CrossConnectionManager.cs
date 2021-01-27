using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SimpleForum.API.Client;
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

        public async Task<Result> RegisterToken(string address)
        {
            // Checks if the address has already been registered
            Result checkResult = await _client.CheckAddress(address, _config.InstanceURL);
            if (checkResult.Failure) return checkResult;
            
            
            // Adds token to repository and 
            OutgoingServerToken token = new OutgoingServerToken()
            {
                Address = address,
                Token = Guid.NewGuid().ToString()
            };

            await _repository.AddOutgoingServerTokenAsync(token);
            await _repository.SaveChangesAsync();
            Result registerResult = await _client.RegisterAddress(address, _config.InstanceURL, token.Token);
            if (registerResult.Failure) return registerResult;
            return Result.Ok();
        }
    }
}