using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;
using SimpleForum.Client.Views;
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
            CreateThreadCommand = new Command(CreateThread);
            RefreshListCommand = new Command(RefreshList);
        }

        public ObservableCollection<Thread> Threads { get; set; } = new ObservableCollection<Thread>();
        public ICommand LoadThreadsCommand { get; set; }
        public ICommand CreateThreadCommand { get; set; }
        public ICommand RefreshListCommand { get; set; }
        
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
            Result<List<ApiThread>> threads = await _account.CurrentInstance.Client.GetFrontPageAsync(currentPage);
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

        // Navigates the user to the create thead page
        private async void CreateThread()
        {
            CreateThreadViewModel model = new CreateThreadViewModel(_account);
            CreateThreadPage page = new CreateThreadPage(model);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
        
        // Refreshes the list of threads
        private void RefreshList()
        {
            // Resets page and retrieves threads
            currentPage = 1;
            Threads.Clear();
            LoadThreads();
        }
    }
}