﻿using System;
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
    public abstract class Post
    {
        public Account Account;
        
        public IApiPost ApiPost { get; set; }
        public FormattedString Content { get; set; } = new FormattedString(); 
        public ImageSource ProfileImage { get; set; }
        public ICommand NavigateUserCommand { get; set; }

        protected Post()
        {
            NavigateUserCommand = new Command(NavigateUser);
        }
        
        // Parsers the text content to a FormattedString
        protected void ParseContent(string content)
        {
            IEnumerable<MarkdownParser.MarkdownValue> markdownValues = MarkdownParser.ParseMarkdown(content);
            Content = XamarinParser.RenderFormattedString(markdownValues);
        }

        // Loads the profile picture
        protected async Task LoadProfileImage()
        {
            Uri imageUri = await Account.CurrentInstance.Client.GetProfileImgUrl(ApiPost.User.ID);
            ProfileImage = new UriImageSource()
            {
                Uri = imageUri,
                CachingEnabled = false
            };
        }

        // Navigates to the user page
        private async void NavigateUser()
        {
            UserViewModel viewModel = new UserViewModel(this);
            UserPage page = new UserPage(viewModel);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
    }
}