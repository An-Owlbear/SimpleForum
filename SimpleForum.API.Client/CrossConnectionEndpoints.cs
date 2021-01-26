using System.Net.Http;

namespace SimpleForum.API.Client
{
    /// <summary>
    /// A class containing cross connection API endpoints
    /// </summary>
    public static class CrossConnectionEndpoints
    {
        public static readonly Endpoint CheckToken = new Endpoint("/Auth/CheckToken", HttpMethod.Get);
        public static readonly Endpoint RegisterToken = new Endpoint("/Auth/RegisterToken", HttpMethod.Put);
    }
}