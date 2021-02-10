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
        /// Get a user by the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<ApiUser>> GetUserAsync(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };
            
            // Receives response and returns result
            HttpResponseMessage response =
                await _requestsClient.SendRequest(Endpoints.GetUser, parameters).ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse<ApiUser>(response);
        }

        /// <summary>
        /// Gets comments from the user of the given ID
        /// </summary>
        /// <param name="id">The ID of the user to retrieve comments for</param>
        /// <param name="page">The page of comments to retrieve</param>
        /// <returns></returns>
        public async Task<Result<List<ApiComment>>> GetUserCommentsAsync(int id, int page = 1)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() },
                { "page", page.ToString() }
            };
            
            // Receives response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.UserComments, parameters)
                .ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse<List<ApiComment>>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Posts a comment to a user profile
        /// </summary>
        /// <param name="id">The ID of the user to post to</param>
        /// <param name="content">The content of the comment</param>
        /// <returns></returns>
        public async Task<Result<ApiComment>> PostUserCommentAsync(int id, string content)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() },
                { "content", content }
            };
            
            // Receives response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.PostUserComment, parameters)
                .ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse<ApiComment>(response).ConfigureAwait(false);
        }
    }
}