using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SimpleForum.Client.ViewModels;

namespace SimpleForum.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            LoginViewModel viewModel = new LoginViewModel();
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}