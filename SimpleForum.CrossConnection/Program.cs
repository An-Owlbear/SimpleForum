using Microsoft.Extensions.Hosting;
using SimpleForum.Common.Server;

namespace SimpleForum.CrossConnection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SimpleForumHostBuilder.CreateHostBuilder<Startup>(args, Service.CrossConnect).Build().Run();
        }
    }
}