using System.Threading.Tasks;
using SimpleForum.API.Client;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;
using SimpleForum.Common;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SimpleForum.Client.ViewModels
{
    public class AddInstanceViewModel
    {
        private readonly Account _account;
        private readonly string _targetInstance;

        public AddInstanceViewModel(Account account, string targetInstance, string location)
        {
            _account = account;
            Location = location;
            _targetInstance = targetInstance;
        }
        
        public string Location { get; set; }

        public async Task OnNavigate(WebView sender, WebNavigatedEventArgs args)
        {
            // Returns if not result URL
            string targetFqdn = _targetInstance.Replace("http://", "").Replace("http://", "");
            if (!args.Url.StartsWith($"http://{targetFqdn}") &&
                !args.Url.StartsWith($"https://{targetFqdn}")) return;

            // Retrieves token and sets it to a new client
            string pageHTML = await sender.EvaluateJavaScriptAsync("document.documentElement.outerHTML").ConfigureAwait(false);
            string response = pageHTML
                .Replace("\\u003Chtml>\\u003Chead>\\u003C/head>\\u003Cbody>\\u003Cpre style=\\\"word-wrap: break-word; white-space: pre-wrap;\\\">", "")
                .Replace("\\u003C/pre>\\u003C/body>\\u003C/html>", "");
            
            // Retrieves server URLs and adds instance
            Result<ServerURLs> serverUrLs = await SimpleForumClient.GetServerURLs(_targetInstance).ConfigureAwait(false);
            if (!this.HandleResult(serverUrLs)) return;
            SimpleForumClient client = new SimpleForumClient(serverUrLs.Value.APIURL);
            client.TokenStorage.SetToken(response);
            Instance instance = new Instance(serverUrLs.Value, client);
            await _account.AddInstance(instance);
            
            // Closes webview
            MainThread.BeginInvokeOnMainThread(async () => await Application.Current.MainPage.Navigation.PopAsync());
        }
    }
}