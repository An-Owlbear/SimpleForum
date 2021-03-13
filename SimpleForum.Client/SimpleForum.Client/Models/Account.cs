using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Services;
using SimpleForum.Client.ViewModels;
using SimpleForum.Client.Views;
using SimpleForum.Common;
using Xamarin.Forms;

namespace SimpleForum.Client.Models
{
    public class Account
    {
        public string Username { get; set; }
        public ServerURLs ServerURLs { get; set; }
        
        [JsonIgnore]
        public SimpleForumClient Client { get; set; }
        
        [JsonIgnore]
        public string Fullname => $"{Username}@{ServerURLs.InstanceURL.Replace("http://", "").Replace("https://", "")}";
        
        [JsonIgnore]
        public ICommand UseUserCommand { get; set; }

        public ObservableCollection<Instance> Instances { get; set; } = new ObservableCollection<Instance>();
        
        [JsonIgnore]
        public Instance CurrentInstance { get; set; }

        public Account(string username, ServerURLs serverURLs, SimpleForumClient client)
        {
            Username = username;
            ServerURLs = serverURLs;
            Client = client;
            UseUserCommand = new Command(UseUser);
            
            Instance instance = new Instance(serverURLs, client);
            CurrentInstance = instance;
            AddInstance(instance).ContinueWith(t => t);
        }

        private async void UseUser()
        {
            InstanceListViewModel viewModel = new InstanceListViewModel(this);
            InstanceListPage page = new InstanceListPage(viewModel);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
        
        public async Task<Result> AuthenticateUser(string password)
        {
            Result<LoginResponse> result = await Client.LoginAsync(Username, password);
            if (result.Failure) return result;
            return Result.Ok();
        }

        public async Task AddInstance(Instance instance)
        {
            Instances.Add(instance);
            await AccountService.SaveAccounts();
        }
    }
}