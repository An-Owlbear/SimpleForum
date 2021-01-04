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
        /// Retrieves the list of threads at the front page
        /// </summary>
        /// <returns>The list of threads</returns>
        public async Task<List<ApiThread>> GetFrontPageAsync(int page = 1)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "page", page.ToString() }
            };
            
            // Retrieves response, converts to a Stream, JsonDocument, and then list of threads
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.FrontPage, parameters).ConfigureAwait(false);
            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            JsonDocument jsonResult = await JsonDocument.ParseAsync(streamResult).ConfigureAwait(false);
            return JsonSerializer.Deserialize<List<ApiThread>>(jsonResult.RootElement.GetRawText(), jsonOptions);
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
            
            // Retrieves response, and converts it to a stream
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.Thread, parameters).ConfigureAwait(false);
            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            // Converts to a thread if the request was successful, otherwise an error
            if (response.IsSuccessStatusCode)
            {
                ApiThread thread = await JsonSerializer.DeserializeAsync<ApiThread>(streamResult, jsonOptions).ConfigureAwait(false);
                return Result.Ok(thread);
            }

            Error error = await JsonSerializer.DeserializeAsync<Error>(streamResult, jsonOptions).ConfigureAwait(false);
            return Result.Fail<ApiThread>(error.Message, error.Type);
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
            
            // Retrieves response, and converts it to a stream
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.CreateThread, parameters).ConfigureAwait(false);
            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            
            // Converts to a thread if successful, otherwise converts to error
            if (response.IsSuccessStatusCode)
            {
                ApiThread thread = await JsonSerializer.DeserializeAsync<ApiThread>(streamResult, jsonOptions).ConfigureAwait(false);
                return Result.Ok(thread);
            }

            Error error = await JsonSerializer.DeserializeAsync<Error>(streamResult, jsonOptions).ConfigureAwait(false);
            return Result.Fail<ApiThread>(error.Message, error.Type);
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
            
            // Retrieves response and converts to stream
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.ThreadComments, parameters).ConfigureAwait(false);
            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            
            // Converts to list of comments if request was successful, otherwise converts to error
            if (response.IsSuccessStatusCode)
            {
                List<ApiComment> comments = await JsonSerializer.DeserializeAsync<List<ApiComment>>(streamResult, jsonOptions).ConfigureAwait(false);
                return Result.Ok(comments);
            }

            Error error = await JsonSerializer.DeserializeAsync<Error>(streamResult, jsonOptions).ConfigureAwait(false);
            return Result.Fail<List<ApiComment>>(error.Message, error.Type);
        }
    }
}