﻿using System.Windows.Input;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.ViewModels;
using SimpleForum.Client.Views;
using Xamarin.Forms;

namespace SimpleForum.Client.Models
{
    public class Thread : Post
    {
        public ApiThread ApiThread { get; set; }
        public ICommand ThreadCommand { get; set; }

        public Thread(ApiThread thread, Account account)
        {
            Account = account;
            ApiThread = thread;
            ApiPost = thread;
            ThreadCommand = new Command(NavigateThread);
            ParseContent(thread.Content);
            LoadProfileImage().ContinueWith(t => t);
        }

        // Navigates the application to the thread
        private async void NavigateThread()
        {
            ThreadViewModel viewModel = new ThreadViewModel(this, Account);
            ThreadPage page = new ThreadPage(viewModel);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
    }
}