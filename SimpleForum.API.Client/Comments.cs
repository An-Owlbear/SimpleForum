using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Internal;

namespace SimpleForum.API.Client
{
    public partial class SimpleForumClient
    {
        /// <summary>
        /// Retrieves a comment of the given ID
        /// </summary>
        /// <param name="id">The ID of the comment to retrieve</param>
        /// <returns></returns>
        public async Task<Result<ApiComment>> GetCommentAsync(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };

            // Retrieves response and converts it to stream 
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.GetComment, parameters).ConfigureAwait(false);
            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            
            // if successful return ApiComment, otherwise returns error
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