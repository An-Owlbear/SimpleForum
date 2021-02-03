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
            _viewModel.LoadThreads().ContinueWith(t => t);
        }
    }
}