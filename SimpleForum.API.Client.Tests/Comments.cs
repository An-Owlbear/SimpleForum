using System;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;

namespace SimpleForum.API.Client.Tests
{
    public partial class Tests
    {
        public static async Task TestGetComment()
        {
            // Receives user input and retrieves comment
            Console.Write("Enter the id of the comment to retrieve\n> ");
            int id = int.Parse(Console.ReadLine());
            Result<ApiComment> result = await client.GetCommentAsync(id);
            
            // Outputs result
            if (result.Success) DisplayItems.DisplayComment(result.Value);
            else DisplayItems.DisplayError(result);
        }
    }
}