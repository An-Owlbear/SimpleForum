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
        }
    }
}