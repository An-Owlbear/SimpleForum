using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SimpleForum.Client.ViewModels;
using SimpleForum.Common;

namespace SimpleForum.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            LoginViewModel viewModel = new LoginViewModel(Navigation);
            BindingContext = viewModel;
            InitializeComponent();
        }

        private async void Return(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void LoginHandler(Result result)
        {
            if (result.Success) await Navigation.PopModalAsync();
            else await DisplayAlert("Error", result.Error, "OK");
        }
    }
}