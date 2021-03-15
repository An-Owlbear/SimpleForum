using SimpleForum.Client.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleForum.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddInstancePage : ContentPage
    {
        private AddInstanceViewModel _viewModel;
        
        public AddInstancePage(AddInstanceViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = viewModel;
            InitializeComponent();
            
            MessagingCenter.Subscribe<AddInstanceViewModel, string>(this, "Error", async (sender, data) =>
            {
                await DisplayAlert("Error", data, "Ok");
            });
        }

        // Calls ViewModel OnNavigation method
        private async void OnNavigated(object sender, WebNavigatedEventArgs args)
        {
            await _viewModel.OnNavigate(sender as WebView, args);
        }
    }
}