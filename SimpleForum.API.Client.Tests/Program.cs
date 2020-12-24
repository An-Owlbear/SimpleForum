using System;
using System.Threading.Tasks;

namespace SimpleForum.API.Client.Tests
{
    static partial class Tests
    {
        private const string separator = "-----------------------------------------------------------------";
        private static readonly SimpleForumClient client = new SimpleForumClient("http://localhost:5002");
        
        static async Task Main()
        {
            while (true)
            {
                // Gets the user's choice
                Console.Write("Enter an option\n" +
                                  "1 - Get front page\n" +
                                  "2 - Get thread\n" +
                                  "3 - Login\n" +
                                  "4 - Create thread\n" +
                                  "> ");

                int choice = int.Parse(Console.ReadLine());
                Console.Clear();

                switch (choice)
                {
                    case 1:
                        await TestFrontPage();
                        break;
                    case 2:
                        await TestThreads();
                        break;
                    case 3:
                        await TestLogin();
                        break;
                    case 4:
                        await TestCreateThread();
                        break;
                }

                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
}