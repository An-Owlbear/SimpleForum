using System;
using System.Collections.Generic;
using System.IO;
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
            // Creates file directories if needed
            CreateDirectories();
            
            // Sets arguments, config and port values
            ServerArguments arguments = ArgumentParser.ParseArguments(args);
            string path = arguments.Config ?? 
                          Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "/SimpleForumConfig.json";
            
            SimpleForumConfig config = new SimpleForumConfig();
            new ConfigurationBuilder()
                .AddJsonFile(path, false)
                .Build()
                .Bind(config);

            int port = service switch
            {
                Service.Web => config.WebPort,
                Service.Api => config.ApiPort,
                Service.CrossConnect => config.CrossConnectionPort,
                _ => throw new ArgumentOutOfRangeException(nameof(service), service, null)
            };

            // Creates HostBuilder
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    configurationBuilder.AddJsonFile(path, false, true);
                    configurationBuilder.AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://localhost:{arguments.Port ?? port}");
                    webBuilder.UseStartup<Startup>();
                });
        }
        
        // Creates the required directories if they don't already exist
        private static void CreateDirectories()
        {
            // List of required directories
            List<string> directories = new List<string>()
            {
                "../UploadedImages",
                "../UploadedImages/ProfilePictures",
                "../UploadedImages/ThreadImages"
            };

            // Creates each directory if needed
            foreach (string directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }
    }
}