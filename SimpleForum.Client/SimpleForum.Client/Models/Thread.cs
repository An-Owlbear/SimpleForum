using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.ViewModels;
using SimpleForum.Client.Views;
using SimpleForum.Common;
using Xamarin.Forms;

namespace SimpleForum.Client.Models
{
    public class Thread : IPost
    {
        private readonly Account _account;
        
        public ApiThread ApiThread { get; set; }
        public IApiPost Post { get; set; }
        public ImageSource ProfileImage { get; set; }
        public ICommand ThreadCommand { get; set; }

        public Thread(ApiThread thread, Account account)
        {
            _account = account;
            ApiThread = thread;
            Post = thread;
            ThreadCommand = new Command(NavigateThread);
            LoadProfileImage().ContinueWith(t => t);
        }

        private async Task LoadProfileImage()
        {
            Result<Stream> result = await _account.CurrentClient.GetProfileImg(Post.User.ID);
            if (result.Success) ProfileImage = ImageSource.FromStream(() => CreateStream(result.Value));
        }

        // Copies stream to a new MemoryStream
        private MemoryStream CreateStream(Stream stream)
        {
            if (stream == null) throw new NullReferenceException();
            byte[] bytes = ((MemoryStream)stream).ToArray();
            return new MemoryStream(bytes);
        }
        
        private async void NavigateThread()
        {
            ThreadViewModel viewModel = new ThreadViewModel(this, _account);
            ThreadPage page = new ThreadPage(viewModel);
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }
    }
}