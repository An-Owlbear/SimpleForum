using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SimpleForum.API.Models.Requests;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.API.Client
{
    public partial class SimpleForumClient
    {
        /// <summary>
        /// Retrieves the list of threads at the front page
        /// </summary>
        /// <returns>The list of threads</returns>
        public async Task<Result<List<ApiThread>>> GetFrontPageAsync(int page = 1)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "page", page.ToString() }
            };
            
            // Retrieves response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.FrontPage, parameters).ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse<List<ApiThread>>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a thread of the given ID
        /// </summary>
        /// <param name="threadID">The thread to retrieve</param>
        /// <returns>The requested thread</returns>
        public async Task<Result<ApiThread>> GetThreadAsync(int threadID)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", threadID.ToString() }
            };
            
            // Retrieves response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.Thread, parameters).ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse<ApiThread>(response);
        }

        /// <summary>
        /// Creates a new thread
        /// </summary>
        /// <param name="title">The title of the new thread</param>
        /// <param name="contents">The contents of the new thread</param>
        /// <returns>The newly created thread/error</returns>
        public async Task<Result<ApiThread>> CreateThreadAsync(string title, string contents)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "title", title },
                { "content", contents }
            };
            
            // Retrieves response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.CreateThread, parameters).ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse<ApiThread>(response);
        }

        /// <summary>
        /// Retrieves a list of comments on a thread
        /// </summary>
        /// <param name="id">The id of the thread</param>
        /// <param name="page">The page of the thread comments</param>
        /// <returns>A list of comments </returns>
        public async Task<Result<List<ApiComment>>> GetThreadCommentsAsync(int id, int page)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() },
                { "page", page.ToString() }
            };
            
            // Retrieves response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.ThreadComments, parameters).ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse<List<ApiComment>>(response);
        }

        /// <summary>
        /// Deletes a thread of the given ID
        /// </summary>
        /// <param name="id">The ID of the thread to delete</param>
        /// <returns></returns>
        public async Task<Result> DeleteThreadAsync(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };
            
            // Retrieves the response and returns result
            HttpResponseMessage response =
                await _requestsClient.SendRequest(Endpoints.DeleteThread, parameters).ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the thread of the given ID as admin
        /// </summary>
        /// <param name="id">The ID of the thread to delete</param>
        /// <returns></returns>
        public async Task<Result> AdminDeleteThreadAsync(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };
            
            // Retrieves response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.AdminDeleteThread, parameters)
                .ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse(response).ConfigureAwait(false);
        }

        public async Task<Result<ApiThread>> AdminUpdateThread(int id, UpdateThreadRequest request)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };
            if (request.Locked != null) parameters.Add("locked", request.Locked.ToString());
            if (request.Pinned != null) parameters.Add("pinned", request.Pinned.ToString());
            
            // Retrieves response and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.AdminUpdateThread, parameters)
                .ConfigureAwait(false);
            return await ResponseParser.ParseJsonResponse<ApiThread>(response).ConfigureAwait(false);
        }
    }
}