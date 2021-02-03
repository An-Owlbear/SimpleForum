using System;
using SimpleForum.Client.ViewModels;
using Xamarin.Forms;

namespace SimpleForum.Client.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;
        
        public MainPage()
        {
            _viewModel = new MainViewModel();
            BindingContext = _viewModel;
            InitializeComponent();
        }

        async void AddAccount(object sender, EventArgs e)
        {
            LoginViewModel loginViewModel = new LoginViewModel(_viewModel.AccountService);
            LoginPage loginPage = new LoginPage(loginViewModel);
            await Navigation.PushModalAsync(loginPage);
        }
    }
}