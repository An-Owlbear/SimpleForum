using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.TextParser;
using Xamarin.Forms;

namespace SimpleForum.Client.Models
{
    public class Comment : Post
    { 
        public ApiComment ApiComment { get; set; }

        public Comment(ApiComment comment, Account account)
        {
            ApiComment = comment;
            ApiPost = comment;
            _account = account;
            ParseContent(comment.Content);
            LoadProfileImage().ContinueWith(t => t);
        }
    }
}