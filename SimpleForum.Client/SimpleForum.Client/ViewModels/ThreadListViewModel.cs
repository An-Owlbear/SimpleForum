using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;

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
            List<ApiThread> threads = await _account.Client.GetFrontPageAsync(page);
            threads.ForEach(x => Threads.Add(new Thread(x, _account)));
        }
    }
}