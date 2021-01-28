using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.API.Client
{
    public class CrossConnectionClient
    {
        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly RequestsClient _requestsClient;

        public CrossConnectionClient()
        {
            _requestsClient = new RequestsClient();
        }
        
        /// <summary>
        /// Checks the given token is registered in the given server
        /// </summary>
        /// <param name="address">The address of the server to query</param>
        /// <param name="token">The token to check for</param>
        /// <returns></returns>
        public async Task<Result> CheckToken(string address, string token)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "token", token }
            };

            HttpResponseMessage response =
                await _requestsClient.SendRequest(address, CrossConnectionEndpoints.CheckToken, parameters)
                    .ConfigureAwait(false);

            if (response.IsSuccessStatusCode) return Result.Ok();

            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            Error error = await JsonSerializer.DeserializeAsync<Error>(streamResult, jsonOptions).ConfigureAwait(false);
            return Result.Fail(error.Message, error.Type);
        }

        /// <summary>
        /// Checks if a domain has already been registered in the given server
        /// </summary>
        /// <param name="address">The server to query</param>
        /// <param name="checkAddress">The address to check</param>
        /// <returns>Returns a successful when it has not been registered, otherwise returns a failure result</returns>
        public async Task<Result> CheckAddress(string address, string checkAddress)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "address", checkAddress }
            };

            HttpResponseMessage response = await _requestsClient
                .SendRequest(address, CrossConnectionEndpoints.CheckAddress, parameters).ConfigureAwait(false);

            if (response.IsSuccessStatusCode) return Result.Ok();

            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            Error error = await JsonSerializer.DeserializeAsync<Error>(streamResult, jsonOptions).ConfigureAwait(false);
            return Result.Fail(error.Message, error.Type);
        }

        /// <summary>
        /// Registers an outgoing token at the given address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="registerAddress"></param>
        /// <param name="token">The token to register on the remote server</param>
        /// <returns></returns>
        public async Task<Result> RegisterAddress(string address, string registerAddress, string token)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "address", registerAddress },
                { "token", token }
            };

            HttpResponseMessage response = await _requestsClient
                .SendRequest(address, CrossConnectionEndpoints.RegisterToken, parameters).ConfigureAwait(false);

            if (response.IsSuccessStatusCode) return Result.Ok();

            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            Error error = await JsonSerializer.DeserializeAsync<Error>(streamResult, jsonOptions).ConfigureAwait(false);
            return Result.Fail(error.Message, error.Type);
        }
    }
}