using SimpleForum.Client.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleForum.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstanceListPage : ContentPage
    {
        public InstanceListPage(InstanceListViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            
            MessagingCenter.Subscribe<InstanceListViewModel, string>(this, "Error", async (sender, data) =>
            {
                await DisplayAlert("Error", data, "Ok");
            });
            
            MessagingCenter.Subscribe<InstanceListViewModel>(this, "add_instance", async (sender) =>
            {
                string response = await DisplayPromptAsync("Add instance", "Add instance URL");
                viewModel.OpenWebviewCommand.Execute(response);
            });
        }
    }
}