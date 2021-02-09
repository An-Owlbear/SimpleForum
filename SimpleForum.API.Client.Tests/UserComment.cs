using System;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.API.Client.Tests
{
    public partial class Tests
    {
        public static async Task TestGetUserComment()
        {
            // Receives user and input and retrieves user comment
            Console.Write("Enter user comment ID\n> ");
            int id = int.Parse(Console.ReadLine());
            Result<ApiComment> result = await client.GetUserCommentAsync(id);
            
            // Outputs result
            if (result.Success) DisplayItems.DisplayComment(result.Value);
            else DisplayItems.DisplayError(result);
        }

        // Tests deleting user comment
        private static async Task TestDeleteUserComment()
        {
            // Receives user input and deletes UserComment
            Console.Write("Enter user comment ID\n> ");
            int id = int.Parse(Console.ReadLine());
            Result result = await client.DeleteUserCommentAsync(id);
            
            // Outputs result
            if (result.Success) Console.WriteLine("Deleted UserComment");
            else DisplayItems.DisplayError(result);
        }

        // Tests deleting user comment as admin
        private static async Task TestAdminDeleteUserComment()
        {
            // Receives user input and deletes UserComment as admin
            Console.Write("Enter user comment ID\n> ");
            int id = int.Parse(Console.ReadLine());
            Result result = await client.AdminDeleteUserCommentAsync(id);
            
            // Outputs result
            if (result.Success) Console.WriteLine("Deleted UserComment");
            else DisplayItems.DisplayError(result);
        }
    }
}