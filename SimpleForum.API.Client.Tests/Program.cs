using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;

namespace SimpleForum.API.Client.Tests
{
    class Program
    {
        // TODO - Add more tests and test selection
        static async Task Main(string[] args)
        {
            SimpleForumClient client = new SimpleForumClient("http://localhost:5002");
            List<ApiThread> response = await client.GetFrontPage();

            foreach (ApiThread thread in response)
            {
                Console.WriteLine($"Title - {thread.Title}\n" +
                                  $"Content - {thread.Content}\n" +
                                  $"Pinned - {thread.Pinned}\n" +
                                  $"Locked - {thread.Locked}\n" +
                                  $"Replies - {thread.Replies}\n");
            }
        }
    }
}