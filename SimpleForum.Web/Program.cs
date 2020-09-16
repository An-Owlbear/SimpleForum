using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SimpleForum.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateDirectories();
            CreateHostBuilder(args).Build().Run();
        }

        // Creates the required directories if they don't already exist
        private static void CreateDirectories()
        {
            // List of required directories
            List<string> directories = new List<string>()
            {
                "UploadedImages",
                "UploadedImages/ProfilePictures",
                "UploadedImages/ThreadImages"
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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}