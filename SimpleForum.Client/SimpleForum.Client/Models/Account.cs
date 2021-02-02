using System.Threading.Tasks;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.Client.Models
{
    public class Account
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public ServerURLs ServerURLs { get; set; }
        public SimpleForumClient Client { get; set; }
        public string Fullname => $"{Username}@{ServerURLs.InstanceURL.Replace("http://", "").Replace("https://", "")}";

        public async Task<Result> AuthenticateUser(string password)
        {
            Result<LoginResponse> result = await Client.LoginAsync(Username, password);
            if (result.Failure) return result;
            Token = result.Value.Token;
            return Result.Ok();
        }
    }
}