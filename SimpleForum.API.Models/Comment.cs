using System;

namespace SimpleForum.API.Models
{
    /// <summary>
    /// A comment object
    /// </summary>
    public class Comment
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public User User { get; set; }

        /// <summary>
        /// Creates an API model comment from a database model comment
        /// </summary>
        /// <param name="comment">The database model comment</param>
        public Comment(SimpleForum.Models.Comment comment)
        {
            ID = comment.CommentID;
            Content = comment.Content;
            DatePosted = comment.DatePosted;
            User = new User(comment.User);
        }
    }
}