using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.TextParser;
using Xamarin.Forms;

namespace SimpleForum.Client.Models
{
    public class Comment : IPost
    {
        private readonly Account _account;

        public ApiComment ApiComment { get; set; }
        public IApiPost Post { get; set; }
        public string Content { get; set; }
        public ImageSource ProfileImage { get; set; }

        public Comment(ApiComment comment, Account account)
        {
            ApiComment = comment;
            Post = comment;
            _account = account;
            Content = ParseContent(comment.Content);
            LoadProfileImage().ContinueWith(t => t);
        }
        
        private string ParseContent(string content)
        {
            IEnumerable<MarkdownParser.MarkdownValue> markdownValues = MarkdownParser.ParseMarkdown(content);
            return MarkdownParser.MarkdownToHTML(markdownValues);
        }
        
        private async Task LoadProfileImage()
        {
            Uri imageUri = await _account.CurrentClient.GetProfileImgUrl(ApiComment.User.ID);
            ProfileImage = ImageSource.FromUri(imageUri);
        }
    }
}