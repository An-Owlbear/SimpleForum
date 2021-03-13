using System.Collections.ObjectModel;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;

namespace SimpleForum.Client.Services
{
    public class AccountService
    {
        public ObservableCollection<Account> Accounts { get; set; } = new ObservableCollection<Account>();

        public void AddAccount(string username, ServerURLs serverURLs, SimpleForumClient client)
        {
            Account account = new Account(username, serverURLs, client);
            Accounts.Add(account);
        }
    }
}
