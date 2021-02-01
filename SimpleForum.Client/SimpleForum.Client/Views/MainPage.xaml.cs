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

        int count = 0;
        void Button_Clicked(object sender, System.EventArgs e)
        {
            count++;
            ((Button)sender).Text = $"You clicked {count} times";
        }

        async void AddAccount(object sender, System.EventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            await Navigation.PushModalAsync(loginPage);
        }
    }
}