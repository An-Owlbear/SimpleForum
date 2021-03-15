namespace SimpleForum.API.Models.Responses
{
    /// <summary>
    /// Represents all the URLs of a server 
    /// </summary>
    public class ServerURLs
    {
        public string InstanceURL { get; set; }
        public string APIURL { get; set; }
        public string CrossConnectionURL { get; set; }
    }
}