using System.Net;
using System.Net.Http;

namespace SimpleForum.API.Client
{
    public class RequestsClient
    {
        private readonly ITokenStorage _tokenStorage;
        private HttpClient _client;

        public RequestsClient(ITokenStorage tokenStorage)
        {
            // Sets token storage
            _tokenStorage = tokenStorage;
            
            // Configures and creates HttpClient
            HttpClientHandler handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }
        
        
    }
}