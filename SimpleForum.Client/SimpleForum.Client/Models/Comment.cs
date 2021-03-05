using SimpleForum.API.Models.Responses;

namespace SimpleForum.Client.Models
{
    public class Comment : Post
    { 
        public ApiComment ApiComment { get; set; }

        public Comment(ApiComment comment, Account account)
        {
            ApiComment = comment;
            ApiPost = comment;
            Account = account;
            ParseContent(comment.Content);
            LoadProfileImage().ContinueWith(t => t);
        }
    }
}