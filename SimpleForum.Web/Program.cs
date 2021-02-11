using Microsoft.Extensions.Hosting;
using SimpleForum.Common.Server;

namespace SimpleForum.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SimpleForumHostBuilder.CreateHostBuilder<Startup>(args, Service.Web).Build().Run();
        }
    }
}