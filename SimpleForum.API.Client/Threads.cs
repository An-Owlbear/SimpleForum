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
        public async Task<List<ApiThread>> GetFrontPage()
        {
            // Retrieves response, converts to a Stream, JsonDocument, and then list of threads
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.FrontPage).ConfigureAwait(false);
            Stream streamResult = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            JsonDocument jsonResult = await JsonDocument.ParseAsync(streamResult).ConfigureAwait(false);
            return JsonSerializer.Deserialize<List<ApiThread>>(jsonResult.RootElement.GetRawText(), jsonOptions);
        }
    }
}