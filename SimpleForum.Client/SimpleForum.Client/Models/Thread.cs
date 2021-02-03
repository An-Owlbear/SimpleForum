using System.Windows.Input;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.ViewModels;
using SimpleForum.Client.Views;
using Xamarin.Forms;

namespace SimpleForum.Client.Models
{
    public class Thread
    {
        private readonly Account _account;

        public ApiThread ApiThread { get; set; }
        public ICommand ThreadCommand { get; set; }

        public Thread(ApiThread thread, Account account)
        {
            _account = account;
            ApiThread = thread;
            ThreadCommand = new Command(NavigateThread);
        }

        private async void NavigateThread()
        {
            ThreadViewModel viewModel = new ThreadViewModel(ApiThread, _account);
            ThreadPage page = new ThreadPage(viewModel);
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }
    }
}