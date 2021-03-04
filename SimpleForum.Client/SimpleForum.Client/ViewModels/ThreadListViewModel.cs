using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;
using SimpleForum.Common;
using Xamarin.Forms;

namespace SimpleForum.Client.ViewModels
{
    public class ThreadListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        private readonly Account _account;
        private int currentPage = 1;
        private bool threadsRemaining = true;

        public ThreadListViewModel(Account account)
        {
            _account = account;
            LoadThreadsCommand = new Command(LoadThreads);
        }

        public ObservableCollection<Thread> Threads { get; set; } = new ObservableCollection<Thread>();
        public ICommand LoadThreadsCommand { get; set; }

        public bool ThreadsRemaining
        {
            get => threadsRemaining;
            set
            {
                threadsRemaining = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ThreadsRemaining"));
            }
        }

        // Loads threads from server
        public async void LoadThreads()
        {
            // Retrieves threads and handles result
            Result<List<ApiThread>> threads = await _account.CurrentClient.GetFrontPageAsync(currentPage);
            if (!this.HandleResult(threads)) return;
            
            // Sets ThreadsRemaining to false if none retrieved
            if (threads.Value.Count == 0)
            {
                ThreadsRemaining = false;
            }
            
            // Adds threads to list and increments page
            threads.Value.ForEach(x => Threads.Add(new Thread(x, _account)));
            currentPage++;
        }
    }
}