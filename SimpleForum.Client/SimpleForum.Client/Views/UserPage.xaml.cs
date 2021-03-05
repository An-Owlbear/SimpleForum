using SimpleForum.Client.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleForum.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage
    {
        public UserPage(UserViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            viewModel.LoadCommentsCommand.Execute(null);
            MessagingCenter.Subscribe<UserViewModel, string>(this, "Error", (sender, data) =>
            {
                DisplayAlert("Error", data, "Ok");
            });
        }
    }
}