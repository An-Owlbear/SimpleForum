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
        public LoginPage(LoginViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            
            MessagingCenter.Subscribe<LoginViewModel, string>(viewModel, "Error", (sender, data) =>
            {
                DisplayAlert("Error", data, "Ok");
            });
        }

        private async void Return(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void LoginHandler(Result result)
        {
            if (result.Success) await Navigation.PopAsync();
            else await DisplayAlert("Error", result.Error, "OK");
        }
    }
}