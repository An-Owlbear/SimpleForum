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
    public class ThreadViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        private readonly Account _account;
        private int currentPage = 1;
        private bool commentsRemaining = true;
        private string replyText;

        public ThreadViewModel(Thread thread, Account account)
        {
            Thread = thread;
            _account = account;
            LoadCommentsCommand = new Command(LoadComments);
            PostCommentCommand = new Command(PostComment);
            RefreshListCommand = new Command(RefreshList);
        }

        public Thread Thread { get; set; }
        public ObservableCollection<Comment> Comments { get; set; } = new ObservableCollection<Comment>();
        public ICommand LoadCommentsCommand { get; set; }
        public ICommand PostCommentCommand { get; set; }
        public ICommand RefreshListCommand { get; set; }
        public bool CommentsRemaining
        {
            get => commentsRemaining;
            set
            {
                commentsRemaining = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CommentsRemaining"));
            }
        }
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
            Result<List<ApiComment>> newComments = await _account.CurrentClient.GetThreadCommentsAsync(Thread.ApiPost.ID, currentPage);
            if (!this.HandleResult(newComments)) return;
            
            // Adds new comments to lists and increments page
            newComments.Value.ForEach(x => Comments.Add(new Comment(x, _account)));
            currentPage++;

            if (Comments.Count == Thread.ApiThread.Replies)
            {
                CommentsRemaining = false;
            }
        }
        
        // Posts a comment at the end of a thread
        private async void PostComment()
        {
            // Posts comment, returning if failed
            Result<ApiComment> comment = await _account.CurrentClient.PostCommentAsync(Thread.ApiPost.ID, ReplyText);
            if (!this.HandleResult(comment)) return;
            
            // Adds comment to list
            Comments.Add(new Comment(comment.Value, _account));
            ReplyText = String.Empty;
        }
        
        // Refreshes the comments
        private async void RefreshList()
        {
            // Resets the page count and comments and retrieves the comments
            currentPage = 1;
            Comments.Clear();
            LoadComments();
        }
    }
}