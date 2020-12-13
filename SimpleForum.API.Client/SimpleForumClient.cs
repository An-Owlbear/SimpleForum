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

        public SimpleForumClient(string fqdn)
        {
            _fqdn = fqdn;
            _requestsClient = new RequestsClient(_fqdn);
        }
    }
}