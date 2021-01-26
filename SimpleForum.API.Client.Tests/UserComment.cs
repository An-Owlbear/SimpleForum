using System;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Internal;

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
    }
}