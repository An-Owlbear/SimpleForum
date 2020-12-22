using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;

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
            List<ApiThread> response = await client.GetFrontPage();
    
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
            Result<ApiThread> response = await client.GetThread(id);
            
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
            Result<ApiThread> result = await client.CreateThread(title, contents);
            
            // Output result
            if (result.Success) DisplayItems.DisplayThread(result.Value);
            else DisplayItems.DisplayError(result);
        }
    }
}