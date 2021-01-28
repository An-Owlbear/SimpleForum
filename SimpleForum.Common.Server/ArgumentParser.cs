﻿using System;
using System.IO;

namespace SimpleForum.Common.Server
{
    public class ArgumentParser
    {
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
                Config = Path.Combine(Directory.GetCurrentDirectory(), config ?? "")
            };
        }
    }
}