using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;

namespace SimpleForum.Client.Services
{
    public static class AccountService
    {
        public static ObservableCollection<Account> Accounts { get; set; } = new ObservableCollection<Account>();

        public static async Task AddAccount(string username, ServerURLs serverURLs, SimpleForumClient client)
        {
            Account account = new Account(username, serverURLs, client);
            Accounts.Add(account);
            await SaveAccounts();
        }

        private static JsonSerializerOptions options = new JsonSerializerOptions()
        {
            IgnoreNullValues = true
            
        };
        
        // Saves the accounts to a json file
        public static async Task SaveAccounts()
        {
            string jsonData = JsonSerializer.Serialize(Accounts, options);
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "data.json");
            await File.WriteAllTextAsync(path, jsonData);
        }

        
        // Loads accounts from the json file
        public static async Task LoadAccounts()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "data.json");
            string jsonData = await File.ReadAllTextAsync(path);
        }
    }
}
