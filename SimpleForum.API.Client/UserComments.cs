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
            return await ResponseParser.ParseJsonResponse<ApiComment>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the UserComment of the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteUserCommentAsync(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };
            
            // Sends request, and returns result
            HttpResponseMessage response = await _requestsClient
                .SendRequest(Endpoints.DeleteUserComment, parameters).ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse(response).ConfigureAwait(false);
        }

        public async Task<Result> AdminDeleteUserCommentAsync(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };
            
            // Sends request, and returns result
            HttpResponseMessage response = await _requestsClient
                .SendRequest(Endpoints.AdminDeleteUserComment, parameters).ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse(response).ConfigureAwait(false);
        }
    }
}