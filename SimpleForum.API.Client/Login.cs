using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

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
            return await Json.ParseHttpResponse<LoginResponse>(response).ConfigureAwait(false);
        }
    }
}