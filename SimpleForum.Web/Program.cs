using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Hosting;
using SimpleForum.Common.Server;

namespace SimpleForum.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateDirectories();
            SimpleForumHostBuilder.CreateHostBuilder<Startup>(args, Service.Web).Build().Run();
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
    }
}