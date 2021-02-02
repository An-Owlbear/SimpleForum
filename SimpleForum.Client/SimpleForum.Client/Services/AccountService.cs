using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;
using SimpleForum.Common;

namespace SimpleForum.Client.Services
{
    public static class AccountService
    {
        public static ObservableCollection<Account> Accounts { get; set; } = new ObservableCollection<Account>();
        public static ObservableCollection<string> Strings { get; set; } = new ObservableCollection<string>();

        public static void AddAccount(string username, string token, ServerURLs serverURLs, SimpleForumClient client)
        {
            Account account = new Account()
            {
                Username = username,
                Token = token,
                ServerURLs = serverURLs
            };
            Accounts.Add(account);
        }
    }
}
