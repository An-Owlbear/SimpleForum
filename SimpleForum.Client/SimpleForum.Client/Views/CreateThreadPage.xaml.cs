using SimpleForum.Client.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleForum.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateThreadPage : ContentPage
    {
        public CreateThreadPage(CreateThreadViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}