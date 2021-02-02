using System.ComponentModel;
using System.Windows.Input;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Services;
using SimpleForum.Common;
using Xamarin.Forms;

namespace SimpleForum.Client.ViewModels
{
    class LoginViewModel : INotifyPropertyChanged
    {
        private INavigation _navigation;

        public LoginViewModel(INavigation navigation)
        {
            _navigation = navigation;
            SubmitCommand = new Command(Login);
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string address;
        private string username;
        private string password;
        private string result;
        private string result2;

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

        public string Result
        {
            get => result;
            set
            {
                result = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
        }
        
        public string Result2
        {
            get => result2;
            set
            {
                result2 = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Result2"));
            }
        }

        public ICommand SubmitCommand { get; set; }

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
                Result = urlsResult.Error;
                return;
            }

            SimpleForumClient client = new SimpleForumClient(urlsResult.Value.APIURL);
            Result<LoginResponse> loginResult = await client.LoginAsync(username, password);
            if (loginResult.Failure)
            {
                Result = loginResult.Error;
                return;
            }

            Result = "OK";
            AccountService.AddAccount(username, loginResult.Value.Token, urlsResult.Value, client);
            await _navigation.PopModalAsync();
        }
    }
}
