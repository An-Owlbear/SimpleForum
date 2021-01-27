using System.Text.Json;

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
    }
}