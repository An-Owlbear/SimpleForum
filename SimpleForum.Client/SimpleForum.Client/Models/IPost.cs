using System.IO;
using SimpleForum.API.Models.Responses;
using Xamarin.Forms;

namespace SimpleForum.Client.Models
{
    public interface IPost
    {
        public IApiPost Post { get; set; }
        public ImageSource ProfileImage { get; set; }
    }
}