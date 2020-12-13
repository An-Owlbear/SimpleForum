using System.Net.Http;

namespace SimpleForum.API.Client
{
    /// <summary>
    /// Represents a single API endpoint
    /// </summary>
    public class Endpoint
    {
        /// <summary>
        /// The location of the endpoint, excluding the fqdn
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// The HTTP method to use
        /// </summary>
        public HttpMethod Method { get; set; }
        
        /// <summary>
        /// Whether the endpoint requires authentication
        /// </summary>
        public bool AuthRequired { get; set; }

        public Endpoint(string path, HttpMethod method, bool authRequired = false)
        {
            Path = path;
            Method = method;
            AuthRequired = authRequired;
        }
    }
}