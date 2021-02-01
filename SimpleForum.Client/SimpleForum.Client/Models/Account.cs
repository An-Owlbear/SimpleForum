using System.Threading.Tasks;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.Client.Models
{
    class Account
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public ServerURLs ServerURLs { get; set; }
        public SimpleForumClient Client { get; set; }

        public async Task<Result> AuthenticateUser(string password)
        {
            Result<LoginResponse> result = await Client.LoginAsync(Username, password);
            if (result.Failure) return result;
            Token = result.Value.Token;
            return Result.Ok();
        }
    }
}