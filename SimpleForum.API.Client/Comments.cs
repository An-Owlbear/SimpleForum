using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

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
            return await ResponseParser.ParseJsonResponse<ApiComment>(response).ConfigureAwait(false);
        }
    }
}