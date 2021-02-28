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
        public FormattedString Content { get; set; } = new FormattedString(); 
        public ImageSource ProfileImage { get; set; }
        
        protected void ParseContent(string content)
        {
            IEnumerable<MarkdownParser.MarkdownValue> markdownValues = MarkdownParser.ParseMarkdown(content);
            Content = XamarinParser.RenderFormattedString(markdownValues);
        }

        protected async Task LoadProfileImage()
        {
            Uri imageUri = await _account.CurrentClient.GetProfileImgUrl(ApiPost.User.ID);
            ProfileImage = ImageSource.FromUri(imageUri);
        }
    }
}