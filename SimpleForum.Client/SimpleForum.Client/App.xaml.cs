using SimpleForum.Client.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SimpleForum.Client.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SimpleForum.Client
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent(); 
            AccountService.LoadAccounts();
            MainPage = new NavigationPage(new MainPage());
        }
    }
}