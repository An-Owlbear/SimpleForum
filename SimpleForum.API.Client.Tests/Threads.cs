using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Internal;

namespace SimpleForum.API.Client.Tests
{
    static partial class Tests
    {
        // Tests retrieving threads from the front page
        private static async Task TestFrontPage()
        {
            // Retrieves list of threads for the given page
            Console.Write("Select a page to view\n> ");
            int page = int.Parse(Console.ReadLine());
            Console.Clear();
            List<ApiThread> response = await client.GetFrontPageAsync();
    
            // Outputs result
            Console.WriteLine(separator);
            foreach (ApiThread thread in response)
            {
                DisplayItems.DisplayThread(thread);
                Console.WriteLine(separator);
            }
        }

        // Tests receiving threads of a given ID
        private static async Task TestThreads()
        {
            // Retrieves threads and displays result
            Console.Write("Enter the ID of the thread to view\n> ");
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            Result<ApiThread> response = await client.GetThreadAsync(id);
            
            // Outputs result
            if (response.Success) DisplayItems.DisplayThread(response.Value);
            else DisplayItems.DisplayError(response);
        }
        
        // Tests creating a thread
        private static async Task TestCreateThread()
        {
            // Receives user input and creates thread
            Console.Write("Thread title: ");
            string title = Console.ReadLine();
            Console.Write("Thread contents: ");
            string contents = Console.ReadLine();
            Result<ApiThread> result = await client.CreateThreadAsync(title, contents);
            
            // Output result
            if (result.Success) DisplayItems.DisplayThread(result.Value);
            else DisplayItems.DisplayError(result);
        }
        
        // Tests retrieving comments of a thread
        private static async Task TestThreadComments()
        {
            // Receives user input and retrieves comment
            Console.Write("Enter the ID of the thread to retrieve\n> ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Enter the page of the comments to view\n> ");
            int page = int.Parse(Console.ReadLine());
            Result<List<ApiComment>> result = await client.GetThreadCommentsAsync(id, page);
            
            // Outputs result
            if (result.Success)
            {
                foreach (ApiComment comment in result.Value) DisplayItems.DisplayComment(comment);
                Console.WriteLine(separator);
            }
            else DisplayItems.DisplayError(result);
        }
    }
}