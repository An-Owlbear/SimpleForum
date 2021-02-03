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
        public string Token { get; set; }
        public ServerURLs ServerURLs { get; set; }
        public SimpleForumClient Client { get; set; }
        public string Fullname => $"{Username}@{ServerURLs.InstanceURL.Replace("http://", "").Replace("https://", "")}";
        public ICommand UseUserCommand { get; set; }

        public Account(string username, string token, ServerURLs serverURLs, SimpleForumClient client)
        {
            Username = username;
            Token = token;
            ServerURLs = serverURLs;
            Client = client;
            UseUserCommand = new Command(UseUser);
        }

        private async void UseUser()
        {
            ThreadListViewModel viewModel = new ThreadListViewModel(this);
            ThreadListPage page = new ThreadListPage(viewModel);
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }
        
        public async Task<Result> AuthenticateUser(string password)
        {
            Result<LoginResponse> result = await Client.LoginAsync(Username, password);
            if (result.Failure) return result;
            Token = result.Value.Token;
            return Result.Ok();
        }
    }
}