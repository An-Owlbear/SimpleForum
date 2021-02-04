using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;
using SimpleForum.Common;

namespace SimpleForum.Client.ViewModels
{
    public class ThreadListViewModel
    {
        private Account _account;

        public ThreadListViewModel(Account account)
        {
            _account = account;
        }
        
        public ObservableCollection<Thread> Threads { get; set; } = new ObservableCollection<Thread>();

        public async Task LoadThreads(int page = 1)
        {
            Threads.Clear();
            Result<List<ApiThread>> threads = await _account.CurrentClient.GetFrontPageAsync(page);
            if (!this.HandleResult(threads)) return;
            threads.Value.ForEach(x => Threads.Add(new Thread(x, _account)));
        }
    }
}