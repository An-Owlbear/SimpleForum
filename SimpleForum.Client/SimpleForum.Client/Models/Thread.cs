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
    public class Thread : IPost
    {
        private readonly Account _account;
        
        public ApiThread ApiThread { get; set; }
        public IApiPost Post { get; set; }
        public string Content { get; set; }
        public ImageSource ProfileImage { get; set; }
        public ICommand ThreadCommand { get; set; }

        public Thread(ApiThread thread, Account account)
        {
            _account = account;
            ApiThread = thread;
            Post = thread;
            Content = ParseContent(thread.Content);
            ThreadCommand = new Command(NavigateThread);
            LoadProfileImage().ContinueWith(t => t);
        }

        private string ParseContent(string content)
        {
            IEnumerable<MarkdownParser.MarkdownValue> markdownValues = MarkdownParser.ParseMarkdown(content);
            return MarkdownParser.MarkdownToHTML(markdownValues);
        }
        
        private async Task LoadProfileImage()
        {
            Uri imageUri = await _account.CurrentClient.GetProfileImgUrl(ApiThread.User.ID);
            ProfileImage = ImageSource.FromUri(imageUri);
        }

        private async void NavigateThread()
        {
            ThreadViewModel viewModel = new ThreadViewModel(this, _account);
            ThreadPage page = new ThreadPage(viewModel);
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }
    }
}