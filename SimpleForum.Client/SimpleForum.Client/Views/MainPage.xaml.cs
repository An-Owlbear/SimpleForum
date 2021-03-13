using System;
using SimpleForum.Client.ViewModels;
using Xamarin.Forms;

namespace SimpleForum.Client.Views
{
    public partial class MainPage : ContentPage
    {
       
        public MainPage()
        {
            InitializeComponent();
        }

        async void AddAccount(object sender, EventArgs e)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            LoginPage loginPage = new LoginPage(loginViewModel);
            await Navigation.PushAsync(loginPage);
        }
    }
}