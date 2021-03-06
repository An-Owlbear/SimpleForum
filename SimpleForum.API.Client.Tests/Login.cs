using System;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.API.Client.Tests
{
    public partial class Tests
    {
        // Tests logging in
        private static async Task TestLogin()
        {
            // User enters username and password
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();
            Console.Clear();

            // Retrieves response and outputs result
            Result<LoginResponse> loginResponse = await client.LoginAsync(username, password);
            if (loginResponse.Failure) DisplayItems.DisplayError(loginResponse);
            else
            {
                Console.WriteLine(separator);
                Console.WriteLine($"Token: {loginResponse.Value.Token}");
                Console.WriteLine($"Valid until: {loginResponse.Value.ValidUntil}");
                Console.WriteLine(separator);
            }
        }
    }
}