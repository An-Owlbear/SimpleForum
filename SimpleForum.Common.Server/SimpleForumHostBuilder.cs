using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SimpleForum.Common.Server
{
    public class SimpleForumHostBuilder
    {
        // Creates a host builder for the given service
        public static IHostBuilder CreateHostBuilder<Startup>(string[] args, Service service) where Startup : class 
        {
            SimpleForumConfig config = new SimpleForumConfig();
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
                .AddJsonFile("SimpleForumConfig.json", false)
                .Build()
                .Bind(config);

            int port = service switch
            {
                Service.Web => config.WebPort,
                Service.Api => config.ApiPort,
                Service.CrossConnect => config.CrossConnectionPort,
                _ => throw new ArgumentOutOfRangeException(nameof(service), service, null)
            };

            string path = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "/SimpleForumConfig.json";

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    configurationBuilder.AddJsonFile(path, false, true);
                    configurationBuilder.AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://localhost:{args.FirstOrDefault() ?? port.ToString()}");
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}