using System.IO;

namespace SimpleForum.Common.Server
{
    public class ArgumentParser
    {
        // Parses arguments for the launching the server
        public static ServerArguments ParseArguments(string[] args)
        {
            int? port = null;
            string config = null;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-p":
                    case "--port":
                        port = int.Parse(args[i + 1]);
                        i++;
                        break;
                    case "-c":
                    case "--config":
                        config = args[i + 1];
                        i++;
                        break;
                }
            }

            return new ServerArguments()
            {
                Port = port,
                Config = config switch
                {
                    null => null,
                    _ => Path.Combine(Directory.GetCurrentDirectory(), config) 
                }
            };
        }
    }
}