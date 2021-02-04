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
        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly RequestsClient _requestsClient;
        private readonly ITokenStorage _tokenStorage;

        public SimpleForumClient(string fqdn, ITokenStorage tokenStorage = null)
        {
            _tokenStorage = tokenStorage ?? new TokenStorage();
            _requestsClient = new RequestsClient(fqdn, _tokenStorage);
        }

        /// <summary>
        /// Gets a list of URLs for the separate services of an instance
        /// </summary>
        /// <param name="address">The address to query</param>
        /// <returns></returns>
        public static async Task<Result<ServerURLs>> GetServerURLs(string address)
        {
            // Instantiates temporary objects
            RequestsClient requestsClient = new RequestsClient();
            Endpoint target = new Endpoint("/Home/InstanceInfo", HttpMethod.Get);

            // Retrieves URL information and converts to stream
            HttpResponseMessage response = await requestsClient.SendRequest(address, target);
            return await ResponseParser.ParseJsonResponse<ServerURLs>(response);
        }
    }
}