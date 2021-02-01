using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;
using SimpleForum.Common;

namespace SimpleForum.Client.Services
{
    class AccountService
    {
        public List<AccountService> Accounts { get; set; }

        public async Task<Result> AddAccount(string username, string url)
        {
            Result<ServerURLs> result = await SimpleForumClient.GetServerURLs(url);
            if (result.Failure) return result;
            Account account = new Account()
            {
                Username = username,
                ServerURLs = result.Value
            };
            return Result.Ok();
        }
    }
}
