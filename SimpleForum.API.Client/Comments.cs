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

            // Retrieves response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.GetComment, parameters).ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse<ApiComment>(response).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Posts a comment to the given thread
        /// </summary>
        /// <param name="threadID">The thread to post the comment to</param>
        /// <param name="content">The comment to post</param>
        /// <returns></returns>
        public async Task<Result<ApiComment>> PostCommentAsync(int threadID, string content)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", threadID.ToString() },
                { "content", content }
            };
            
            // Retrieves response and returns result
            HttpResponseMessage response =
                await _requestsClient.SendRequest(Endpoints.PostComment, parameters).ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse<ApiComment>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the comment of the given ID
        /// </summary>
        /// <param name="id">The ID of the comment to delete</param>
        /// <returns></returns>
        public async Task<Result> DeleteCommentAsync(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };
            
            // Retrieves response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.DeleteComment, parameters)
                .ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the comment of the given ID as an admin
        /// </summary>
        /// <param name="id">The comment to delete</param>
        /// <returns></returns>
        public async Task<Result> AdminDeleteCommentAsync(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };
            
            // Retrieves response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.AdminDeleteComment, parameters)
                .ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse(response).ConfigureAwait(false);
        }
    }
}