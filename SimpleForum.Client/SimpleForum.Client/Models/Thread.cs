using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.ViewModels;
using SimpleForum.Client.Views;
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
            Uri imageUri = await _account.CurrentClient.GetProfileImgUrl(ApiThread.ID);
            ProfileImage = ImageSource.FromUri(imageUri);
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