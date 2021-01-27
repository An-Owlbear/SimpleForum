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
        /// Retrieves a user comment of the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<ApiComment>> GetUserCommentAsync(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };
            
            // Sends request, and converts response to stream
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.GetUserComment, parameters).ConfigureAwait(false);
            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            
            // If successful, converts stream to ApiComment, otherwise converts to error
            if (response.IsSuccessStatusCode)
            {
                ApiComment comment = await JsonSerializer.DeserializeAsync<ApiComment>(streamResult, jsonOptions).ConfigureAwait(false);
                return Result.Ok(comment);
            }

            Error error = await JsonSerializer.DeserializeAsync<Error>(streamResult, jsonOptions).ConfigureAwait(false);
            return Result.Fail<ApiComment>(error.Message, error.Type);
        }
    }
}