using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.API.Client.Tests
{
    public partial class Tests
    {
        // Test retrieving a user
        private static async Task TestGetUser()
        {
            // Retrieves user input and gets user
            Console.Write("Enter the user ID\n> ");
            int id = int.Parse(Console.ReadLine());
            Result<ApiUser> result = await client.GetUserAsync(id);
            
            // Outputs result
            if (result.Success) DisplayItems.DisplayUser(result.Value);
            else DisplayItems.DisplayError(result);
        }

        // Tests getting user comments
        private static async Task TestGetUserComments()
        {
            // Retrieves user input and gets user comments
            Console.Write("Enter the user ID\n> ");
            int id = int.Parse(Console.ReadLine());
            Result<List<ApiComment>> result = await client.GetUserCommentsAsync(id);
            
            // Outputs result
            if (result.Success) result.Value.ForEach(DisplayItems.DisplayComment);
            else DisplayItems.DisplayError(result);
        }
        
        // Tests posting a user comment
        private static async Task TestPostUserComment()
        {
            // Retrieves user input and posts comment
            Console.Write("Enter the user ID\n> ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Enter the content to post\n> ");
            string content = Console.ReadLine();
            Result<ApiComment> result = await client.PostUserCommentAsync(id, content);
            
            // Outputs result
            if (result.Success) DisplayItems.DisplayComment(result.Value);
            else DisplayItems.DisplayError(result);
        }
    }
}