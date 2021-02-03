using SimpleForum.API.Models.Responses;
using SimpleForum.Client.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleForum.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThreadPage : ContentPage
    {
        public ThreadPage(ThreadViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}