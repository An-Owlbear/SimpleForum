using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.ViewModels;
using SimpleForum.Client.Views;
using SimpleForum.TextParser;
using Xamarin.Forms;

namespace SimpleForum.Client.Models
{
    public class Thread : Post
    {
        private readonly Account _account;
        
        public ApiThread ApiThread { get; set; }
        public ICommand ThreadCommand { get; set; }

        public Thread(ApiThread thread, Account account)
        {
            _account = account;
            ApiThread = thread;
            ApiPost = thread;
            Content = ParseContent(thread.Content);
            ThreadCommand = new Command(NavigateThread);
            LoadProfileImage().ContinueWith(t => t);
        }

        private async void NavigateThread()
        {
            ThreadViewModel viewModel = new ThreadViewModel(this, _account);
            ThreadPage page = new ThreadPage(viewModel);
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }
    }
}