using Microsoft.Extensions.Hosting;
using SimpleForum.Internal;

namespace SimpleForum.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SimpleForumHostBuilder.CreateHostBuilder<Startup>(args, Service.Api).Build().Run();
        }
    }
}