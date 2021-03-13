using System.Windows.Input;
using SimpleForum.Client.Models;
using SimpleForum.Client.Views;
using SimpleForum.Common;
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
            AddInstanceCommand = new Command(AddInstance);
            OpenWebviewCommand = new Command<string>(OpenWebview);
        }
        
        public ICommand NavigateInstanceCommand { get; set; }
        public ICommand AddInstanceCommand { get; set; }
        public ICommand OpenWebviewCommand { get; set; }

        // Navigates to the selected instance
        private async void NavigateInstance(Instance instance)
        {
            Account.CurrentInstance = instance;
            ThreadListViewModel viewModel = new ThreadListViewModel(Account);
            ThreadListPage page = new ThreadListPage(viewModel);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

        // Activates a popup for adding an instance
        private void AddInstance()
        {
            MessagingCenter.Send(this, "add_instance");
        }

        // Opens the webview for sign in
        private async void OpenWebview(string targetInstance)
        {
            if (string.IsNullOrEmpty(targetInstance)) return;
            if (!targetInstance.StartsWith("http://") && !targetInstance.StartsWith("https://")) targetInstance = $"http://{targetInstance}";

            // Authentication token is generated and retrieved, returning error if unsuccessful
            Result<string> token = await Account.Client.GenerateTokenAsync();
            if (!this.HandleResult(token)) return;
            string location = $"{Account.ServerURLs.InstanceURL}/Login/ApiCrossLogin?address={targetInstance}&token={token.Value}";
            
            // Navigates to webview
            AddInstanceViewModel viewModel = new AddInstanceViewModel(Account, targetInstance, location);
            AddInstancePage page = new AddInstancePage(viewModel);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
    }
}