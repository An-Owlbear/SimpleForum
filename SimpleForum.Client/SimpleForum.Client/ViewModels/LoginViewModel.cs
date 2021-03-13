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
            string url = (address.StartsWith("http://") || address.StartsWith("https://")) switch
            {
                true => address,
                false => $"http://{address}"
            };
            
            Result<ServerURLs> urlsResult = await SimpleForumClient.GetServerURLs(url);
            if (urlsResult.Failure)
            {
                MessagingCenter.Send(this, "Error", urlsResult.Error);
                return;
            }

            SimpleForumClient client = new SimpleForumClient(urlsResult.Value.APIURL);
            Result<LoginResponse> loginResult = await client.LoginAsync(username, password);
            if (loginResult.Failure)
            {
                MessagingCenter.Send(this, "Error", loginResult.Error);
                return;
            }
            
            await AccountService.AddAccount(username, urlsResult.Value, client);
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
