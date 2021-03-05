using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SimpleForum.API.Models.Responses;
using SimpleForum.Client.Models;
using SimpleForum.Common;
using Xamarin.Forms;

namespace SimpleForum.Client.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private readonly Account _account;
        private int currentPage = 1;
        
        public ApiUser ApiUser { get; set; }
        public ImageSource ProfileImage { get; set; }

        public UserViewModel(Post post)
        {
            ApiUser = post.ApiPost.User;
            ProfileImage = post.ProfileImage;
            _account = post.Account;
            LoadCommentsCommand = new Command(LoadComments);
            PostCommentCommand = new Command(PostComment);
            RefreshListCommand = new Command(RefreshList);
        }

        public ObservableCollection<Comment> Comments { get; set; } = new ObservableCollection<Comment>();
        public ICommand LoadCommentsCommand { get; set; }
        public ICommand PostCommentCommand { get; set; }
        public ICommand RefreshListCommand { get; set; }

        private bool commentsRemaining = true;
        public bool CommentsRemaining
        {
            get => commentsRemaining;
            set
            {
                commentsRemaining = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CommentsRemaining"));
            }
        }

        private string replyText;

        public string ReplyText
        {
            get => replyText;
            set
            {
                replyText = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ReplyText"));
            }
        }

        // Loads the next page of comments
        private async void LoadComments()
        {
            // Requests comments, returning if failed
            Result<List<ApiComment>> result = await _account.CurrentClient.GetUserCommentsAsync(ApiUser.ID, currentPage);
            if (!this.HandleResult(result)) return;
            
            // If there are no replies set CommentsRemaining
            if (result.Value.Count == 0)
            {
                CommentsRemaining = false;
                return;
            }
            
            // Adds comments to list and increments currentPage
            result.Value.ForEach(x => Comments.Add(new Comment(x, _account)));
            currentPage++;
        }

        private async void PostComment()
        {
            // Posts comment, returning if failed
            Result<ApiComment> result = await _account.CurrentClient.PostUserCommentAsync(ApiUser.ID, ReplyText);
            if (!this.HandleResult(result)) return;
            
            // Refreshes list of comments
            RefreshList();
        }

        private void RefreshList()
        {
            // Resets the page count and comments and retrieves the comments
            currentPage = 1;
            Comments.Clear();
            LoadComments();
        }
    }
}