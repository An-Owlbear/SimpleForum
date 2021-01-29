using System;

namespace SimpleForum.API.Models.Responses
{
    /// <summary>
    /// A comment object
    /// </summary>
    public class ApiComment
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public ApiUser User { get; set; }

        // Parameterless constructor for use with json deserialization
        public ApiComment() { }
        
        /// <summary>
        /// Creates an API model comment from a database model comment
        /// </summary>
        /// <param name="comment">The database model comment</param>
        public ApiComment(SimpleForum.Models.Comment comment)
        {
            ID = comment.CommentID;
            Type = "Comment";
            Content = comment.Content;
            DatePosted = comment.DatePosted;
            User = new ApiUser(comment.User);
        }

        /// <summary>
        /// Creates an API model comment from a database model UserComment
        /// </summary>
        /// <param name="comment">The database model UserComment</param>
        public ApiComment(SimpleForum.Models.UserComment comment)
        {
            ID = comment.UserCommentID;
            Type = "UserComment";
            Content = comment.Content;
            DatePosted = comment.DatePosted;
            User = new ApiUser(comment.User);
        }
    }
}