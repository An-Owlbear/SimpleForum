using System.Text.Json.Serialization;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;

namespace SimpleForum.Client.Models
{
    public class Instance
    {
        public ServerURLs ServerUrLs { get; set; }
        
        [JsonIgnore]
        public SimpleForumClient Client { get; set; }

        public string ServerName
        {
            get => ServerUrLs.InstanceURL.Replace("https://", "").Replace("http://", "");
        }

        public Instance(ServerURLs serverUrLs, SimpleForumClient client)
        {
            ServerUrLs = serverUrLs;
            Client = client;
        }
    }
}