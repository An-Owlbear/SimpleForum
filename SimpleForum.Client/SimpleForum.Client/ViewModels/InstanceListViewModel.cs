using System;
using System.Windows.Input;
using SimpleForum.Client.Models;
using SimpleForum.Client.Views;
using Xamarin.Forms;

namespace SimpleForum.Client.ViewModels
{
    public class InstanceListViewModel
    {
        public Account Account { get; set; }

        public InstanceListViewModel(Account account)
        {
            Account = account;
            NavigateInstanceCommand = new Command<Instance>(NavigateInstance);
        }
        
        public ICommand NavigateInstanceCommand { get; set; }

        // Navigates to the selected instance
        private async void NavigateInstance(Instance instance)
        {
            Account.CurrentClient = instance.Client;
            ThreadListViewModel viewModel = new ThreadListViewModel(Account);
            ThreadListPage page = new ThreadListPage(viewModel);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
    }
}