using System;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.API.Client.Tests
{
    public partial class Tests
    {
        private static async Task TestGetComment()
        {
            // Receives user input and retrieves comment
            Console.Write("Enter the id of the comment to retrieve\n> ");
            int id = int.Parse(Console.ReadLine());
            Result<ApiComment> result = await client.GetCommentAsync(id);
            
            // Outputs result
            if (result.Success) DisplayItems.DisplayComment(result.Value);
            else DisplayItems.DisplayError(result);
        }

        // Tests deleting comments
        private static async Task TestDeleteComment()
        {
            // Receives user input and deletes comment
            Console.Write("Enter the id of the comment to delete\n> ");
            int id = int.Parse(Console.ReadLine());
            Result result = await client.DeleteCommentAsync(id);
            
            // Outputs result
            if (result.Success) Console.WriteLine("Comment deleted");
            else DisplayItems.DisplayError(result);
        }

        // Tests deleting comments as admin
        private static async Task TestAdminDeleteComment()
        {
            // Receives user input and deletes comment as admin
            Console.Write("Enter the id of the comment to delete\n> ");
            int id = int.Parse(Console.ReadLine());
            Result result = await client.AdminDeleteCommentAsync(id);
            
            // Outputs result
            if (result.Success) Console.WriteLine("Comment deleted");
            else DisplayItems.DisplayError(result);
        }
        
        // Tests posting a comment
        private static async Task TestPostComment()
        {
            // Receives user input and posts comment
            Console.Write("Enter the ID of the post to comment on\n> ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Enter the contents of the comment to post\n> ");
            string content = Console.ReadLine();
            Result<ApiComment> result = await client.PostCommentAsync(id, content);
            
            // Outputs result
            if (result.Success) DisplayItems.DisplayComment(result.Value);
            else DisplayItems.DisplayError(result);
        }
    }
}