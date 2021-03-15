namespace SimpleForum.Common.Server
{
    // Contains information of the server config
    public class SimpleForumConfig
    {
        public string InstanceName { get; set; }
        public string InstanceURL { get; set; }
        public string APIURL { get; set; }
        public string CrossConnectionURL { get; set; }
        public bool RequireEmailVerification { get; set; }
        public string PrivateKey { get; set; }
        public int WebPort { get; set; }
        public int ApiPort { get; set; }
        public int CrossConnectionPort { get; set; }
    }
}