using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SimpleForum.Client.ViewModels;

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

        // Pops view, returning to previous
        private async void Return(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}