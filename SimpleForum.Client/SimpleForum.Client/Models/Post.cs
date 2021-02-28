using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.TextParser;
using Xamarin.Forms;

namespace SimpleForum.Client.Models
{
    public abstract class Post
    {
        protected Account _account;
        
        public IApiPost ApiPost { get; set; }
        public string Content { get; set; }
        public ImageSource ProfileImage { get; set; }
        
        protected string ParseContent(string content)
        {
            IEnumerable<MarkdownParser.MarkdownValue> markdownValues = MarkdownParser.ParseMarkdown(content);
            return MarkdownParser.MarkdownToHTML(markdownValues);
        }
        
        protected async Task LoadProfileImage()
        {
            Uri imageUri = await _account.CurrentClient.GetProfileImgUrl(ApiPost.User.ID);
            ProfileImage = ImageSource.FromUri(imageUri);
        }
    }
}