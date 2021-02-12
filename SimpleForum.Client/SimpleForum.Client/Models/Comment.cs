using System.Collections.Generic;
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
            IEnumerable<MarkdownParser.MarkdownValue> markdownValues = MarkdownParser.ParseMarkdown(Post.Content);
            Content = MarkdownParser.MarkdownToHTML(markdownValues);
            
            ApiComment = comment;
            Post = comment;
            _account = account;
        }
    }
}