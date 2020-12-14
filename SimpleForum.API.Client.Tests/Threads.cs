using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;

namespace SimpleForum.API.Client.Tests
{
    static partial class Tests
    {
        public static async Task TestFrontPage()
        {
            // Retrieves list of threads for the given page
            Console.Write("Select a page to view\n> ");
            int page = int.Parse(Console.ReadLine());
            SimpleForumClient client = new SimpleForumClient("http://localhost:5002");
            List<ApiThread> response = await client.GetFrontPage();
    
            // Outputs result
            foreach (ApiThread thread in response)
            {
                Console.WriteLine($"Title - {thread.Title}\n" +
                                  $"Content - {thread.Content}\n" +
                                  $"Pinned - {thread.Pinned}\n" +
                                  $"Locked - {thread.Locked}\n" +
                                  $"Replies - {thread.Replies}\n" +
                                  separator);
            }
        }
    }
}