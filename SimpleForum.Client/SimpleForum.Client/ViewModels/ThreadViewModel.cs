using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;
using SimpleForum.Common;

namespace SimpleForum.Client.ViewModels
{
    public class ThreadViewModel
    {
        private Account _account;

        public ThreadViewModel(Thread thread, Account account)
        {
            Thread = thread;
            _account = account;
        }

        public Thread Thread { get; set; }
        public ObservableCollection<Comment> Comments { get; set; } = new ObservableCollection<Comment>();

        private async Task LoadComments()
        {
            Result<List<ApiComment>> newComments = await _account.CurrentClient.GetThreadCommentsAsync(Thread.Post.ID, 1);
            if (!this.HandleResult(newComments)) return;
            newComments.Value.ForEach(x => Comments.Add(new Comment(x, _account)));
        }
    }
}