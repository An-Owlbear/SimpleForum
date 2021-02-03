using SimpleForum.Client.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleForum.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThreadListPage : ContentPage
    {
        private ThreadListViewModel _viewModel;
        
        public ThreadListPage(ThreadListViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await _viewModel.LoadThreads();
            base.OnAppearing();
        }
    }
}