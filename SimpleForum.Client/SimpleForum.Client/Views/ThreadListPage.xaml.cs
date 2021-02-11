using SimpleForum.Client.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleForum.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThreadListPage : ContentPage
    {
        private readonly ThreadListViewModel _viewModel;
        
        public ThreadListPage(ThreadListViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;
            InitializeComponent();
            
            MessagingCenter.Subscribe<ThreadListViewModel, string>(_viewModel, "Error", (sender, data) =>
            {
                DisplayAlert("Error", data, "Ok");
            });

            _viewModel.LoadThreads().ContinueWith(t => t);
        }
    }
}