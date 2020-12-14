using System;
using System.Threading.Tasks;

namespace SimpleForum.API.Client.Tests
{
    static partial class Tests
    {
        private const string separator = "-----------------------------------------------------------------";
        
        static async Task Main()
        {
            while (true)
            {
                // Gets the user's choice
                Console.Write("Enter an option\n" +
                                  "1 - Get front page\n" +
                                  "> ");

                int choice = int.Parse(Console.ReadLine());
                Console.Clear();

                switch (choice)
                {
                    case 1:
                        await TestFrontPage();
                        break;
                }

                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
}