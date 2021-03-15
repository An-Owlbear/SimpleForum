using System;
using System.Threading.Tasks;

namespace SimpleForum.API.Client.Tests
{
    static partial class Tests
    {
        private const string separator = "-----------------------------------------------------------------";
        private static readonly SimpleForumClient client = new SimpleForumClient("http://localhost:5001");
        
        // Runs a test of the user's choice
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
                                  "5 - Get thread comments\n" +
                                  "6 - Get comment\n" +
                                  "7 - Get UserComment\n" +
                                  "8 - Download a profile picture\n" +
                                  "9 - Delete thread\n" +
                                  "10 - Admin delete thread\n" +
                                  "11 - Delete comment\n" +
                                  "12 - Admin delete comment\n" +
                                  "13 - Delete UserComment\n" +
                                  "14 - Admin delete UserComment\n" +
                                  "15 - Get user\n" +
                                  "16 - Get UserComments\n" +
                                  "17 - Post comment\n" +
                                  "18 - Post UserComment\n" +
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
                    case 5:
                        await TestThreadComments();
                        break;
                    case 6:
                        await TestGetComment();
                        break;
                    case 7:
                        await TestGetUserComment();
                        break;
                    case 8:
                        await TestProfileImage();
                        break;
                    case 9:
                        await TestDeleteThread();
                        break;
                    case 10:
                        await TestDeleteThreadAdmin();
                        break;
                    case 11:
                        await TestDeleteComment();
                        break;
                    case 12:
                        await TestAdminDeleteComment();
                        break;
                    case 13:
                        await TestDeleteUserComment();
                        break;
                    case 14:
                        await TestAdminDeleteUserComment();
                        break;
                    case 15:
                        await TestGetUser();
                        break;
                    case 16:
                        await TestGetUserComments();
                        break;
                    case 17:
                        await TestPostComment();
                        break;
                    case 18:
                        await TestPostUserComment();
                        break;
                }

                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
}