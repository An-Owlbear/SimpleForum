using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
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
        public SimpleForumClient Client { get; set; }
        public string Fullname => $"{Username}@{ServerURLs.InstanceURL.Replace("http://", "").Replace("https://", "")}";
        public ICommand UseUserCommand { get; set; }

        public ObservableCollection<Instance> Instances { get; set; } = new ObservableCollection<Instance>();
        public Instance CurrentInstance { get; set; }

        public Account(string username, ServerURLs serverURLs, SimpleForumClient client)
        {
            Username = username;
            ServerURLs = serverURLs;
            Client = client;
            UseUserCommand = new Command(UseUser);
            
            Instance instance = new Instance(serverURLs, client);
            Instances.Add(instance);
            CurrentInstance = instance;
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
    }
}