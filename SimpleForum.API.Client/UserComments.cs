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
            return await Json.ParseHttpResponse<ApiComment>(response).ConfigureAwait(false);
        }
    }
}