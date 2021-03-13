using System.Windows.Input;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;
using SimpleForum.Common;
using Xamarin.Forms;

namespace SimpleForum.Client.ViewModels
{
    public class CreateThreadViewModel
    {
        private readonly Account _account;

        public CreateThreadViewModel(Account account)
        {
            _account = account;
            CreateThreadCommand = new Command(CreateThread);
        }
        
        public ICommand CreateThreadCommand { get; set; }
        
        public string Title { get; set; }
        public string Content { get; set; }

        // Creates a thread of the given title and content
        private async void CreateThread()
        {
            // Sends request and returns if unsuccessful
            Result<ApiThread> result = await _account.CurrentInstance.Client.CreateThreadAsync(Title, Content);
            if (!this.HandleResult(result)) return;

            // Navigates to the thread
            await Application.Current.MainPage.Navigation.PopAsync();
            Thread thread = new Thread(result.Value, _account);
            thread.ThreadCommand.Execute(null);
        }
    }
}