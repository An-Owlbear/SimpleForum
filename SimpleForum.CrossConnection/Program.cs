using Microsoft.Extensions.Hosting;
using SimpleForum.Internal;

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