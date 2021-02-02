using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            LoginPage loginPage = new LoginPage();
            await Navigation.PushModalAsync(loginPage);
        }
    }
}