using System.Text.Json;

namespace SimpleForum.API.Client
{
    public partial class SimpleForumClient
    {
        private string _fqdn;

        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly RequestsClient _requestsClient;
        private readonly ITokenStorage _tokenStorage;

        public SimpleForumClient(string fqdn, ITokenStorage tokenStorage = null)
        {
            _fqdn = fqdn;
            _requestsClient = new RequestsClient(_fqdn);
            _tokenStorage = tokenStorage ?? new TokenStorage();
        }
    }
}