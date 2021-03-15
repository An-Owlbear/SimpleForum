using Microsoft.Extensions.Hosting;
using SimpleForum.Common.Server;

namespace SimpleForum.API
{
    public class Program
    {
        // Starts the API service
        public static void Main(string[] args)
        {
            SimpleForumHostBuilder.CreateHostBuilder<Startup>(args, Service.Api).Build().Run();
        }
    }
}