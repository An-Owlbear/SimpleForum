using System.Collections.ObjectModel;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;

namespace SimpleForum.Client.ViewModels
{
    public class ThreadViewModel
    {
        private Account _account;

        public ThreadViewModel(ApiThread thread, Account account)
        {
            Thread = thread;
            _account = account;
        }

        public ApiThread Thread;
        public ObservableCollection<ApiComment> Comments { get; set; } = new ObservableCollection<ApiComment>();
    }
}