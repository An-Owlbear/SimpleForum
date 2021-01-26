using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleForum.API.Client
{
    public class RequestsClient
    {
        private readonly HttpClient _client;
        private readonly ITokenStorage _tokenStorage;
        private readonly string _fqdn;

        public RequestsClient(string fqdn) : this()
        {
            _fqdn = fqdn;
        }

        public RequestsClient(ITokenStorage tokenStorage) : this()
        {
            _tokenStorage = tokenStorage;
        }
        
        public RequestsClient(string fqdn, ITokenStorage tokenStorage) : this()
        {
            _fqdn = fqdn;
            _tokenStorage = tokenStorage;
        }
        
        public RequestsClient()
        {
            // Configures and creates HttpClient
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }

        public async Task<HttpResponseMessage> SendRequest(Endpoint endpoint, Dictionary<string, string> parameters = null, string auth = null)
        {
            // Throws exception if _fqdn is null
            if (String.IsNullOrEmpty(_fqdn)) throw new InvalidOperationException();

            return await SendRequest(_fqdn, endpoint, parameters);
        }

        public async Task<HttpResponseMessage> SendRequest(string fqdn, Endpoint endpoint, Dictionary<string, string> parameters = null, string auth = null)
        { 
            // Sets parameters as an empty dictionary if null
            parameters ??= new Dictionary<string, string>();
            
            // Creates request
            HttpRequestMessage request = new HttpRequestMessage { Method = endpoint.Method };

            // Creates a list of parameters in the url path
            Dictionary<string, string> pathParameters = parameters
                .Where(x => Regex.IsMatch(endpoint.Path, $@":{x.Key}(?!\w)"))
                .ToDictionary(x => x.Key, x => x.Value);

            // Creates a list of remaining parameters
            Dictionary<string, string> remainingParams = parameters
                .Except(pathParameters)
                .ToDictionary(x => x.Key, x => x.Value);

            // Creates url string
            string url = fqdn + pathParameters
                .Aggregate(endpoint.Path,
                    (acc, next) =>
                        Regex.Replace(acc, $@":{next.Key}(?!\w)", next.Value));

            // adds parameters to request
            if (request.Method == HttpMethod.Get)
            {
                UriBuilder address = new UriBuilder(url);
                address.Query = await new FormUrlEncodedContent(remainingParams).ReadAsStringAsync().ConfigureAwait(false);
                request.RequestUri = new Uri(address.ToString());
            }
            else
            {
                request.RequestUri = new Uri(url);
                string jsonBody = JsonSerializer.Serialize(remainingParams);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            // Adds authentication
            if (endpoint.AuthRequired)
            {
                request.Headers.Add("Authorization", $"Bearer {auth ?? _tokenStorage.GetToken()}");
            }

            // Returns response
            return await _client.SendAsync(request).ConfigureAwait(false);
        }
    }
}