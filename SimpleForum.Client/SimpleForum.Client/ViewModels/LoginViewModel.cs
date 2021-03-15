using System.ComponentModel;
using System.Windows.Input;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Services;
using SimpleForum.Common;
using Xamarin.Forms;

namespace SimpleForum.Client.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string address;
        private string username;
        private string password;

        public string Address
        {
            get => address;
            set
            {
                address = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Address"));
            }
        }

        public string Username
        {
            get => username;
            set
            {
                username = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Username"));
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
            }
        }

        public ICommand SubmitCommand { get; set; }
        
        public LoginViewModel()
        {
            SubmitCommand = new Command(Login);
        }

        private async void Login()
        {
            // Sets url protocol if missing
            string url = (address.StartsWith("http://") || address.StartsWith("https://")) switch
            {
                true => address,
                false => $"http://{address}"
            };
            
            // Retrieves server URLs, returning if unsuccessful
            Result<ServerURLs> urlsResult = await SimpleForumClient.GetServerURLs(url);
            if (urlsResult.Failure)
            {
                MessagingCenter.Send(this, "Error", urlsResult.Error);
                return;
            }

            // Creates client, and logs in, returning if unsuccessful
            SimpleForumClient client = new SimpleForumClient(urlsResult.Value.APIURL);
            Result<LoginResponse> loginResult = await client.LoginAsync(username, password);
            if (loginResult.Failure)
            {
                MessagingCenter.Send(this, "Error", loginResult.Error);
                return;
            }
            
            // Adds account and pops view
            await AccountService.AddAccount(username, urlsResult.Value, client);
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
