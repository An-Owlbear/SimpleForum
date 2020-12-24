using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;

namespace SimpleForum.API.Client
{
    public partial class SimpleForumClient
    {
        /// <summary>
        /// Logs the user in, returning an access token
        /// </summary>
        /// <param name="username">The username to use</param>
        /// <param name="password">The password to use</param>
        /// <returns>The LoginResponse containing the access token</returns>
        public async Task<Result<LoginResponse>> LoginAsync(string username, string password)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "username", username },
                { "password", password }
            };
            
            // Sends login, and converts response to stream
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.Login, parameters).ConfigureAwait(false);
            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            
            // Converts to a LoginResponse if successful, otherwise converts to an error
            if (response.IsSuccessStatusCode)
            {
                LoginResponse loginResponse = await JsonSerializer.DeserializeAsync<LoginResponse>(streamResult, jsonOptions).ConfigureAwait(false);
                _tokenStorage.SetToken(loginResponse.Token);
                return Result.Ok(loginResponse);
            }

            Error error = await JsonSerializer.DeserializeAsync<Error>(streamResult, jsonOptions).ConfigureAwait(false);
            return Result.Fail<LoginResponse>(error.Message, error.Type);
        }
    }
}