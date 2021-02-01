using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleForum.Client.ViewModels
{
    class LoginViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        private string username;
        private string password;

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Username"));
            }
        }

        public string Password
        {
            get { return password; }
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

        public void Login()
        {

        }
    }
}
