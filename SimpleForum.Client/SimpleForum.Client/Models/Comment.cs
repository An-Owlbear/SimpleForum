using SimpleForum.API.Models.Responses;
using Xamarin.Forms;

namespace SimpleForum.Client.Models
{
    public class Comment : IPost
    {
        private readonly Account _account;

        public ApiComment ApiComment { get; set; }
        public IApiPost Post { get; set; }
        public ImageSource ProfileImage { get; set; }

        public Comment(ApiComment comment, Account account)
        {
            ApiComment = comment;
            Post = comment;
            _account = account;
        }
    }
}