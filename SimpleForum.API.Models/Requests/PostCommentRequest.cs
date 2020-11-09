namespace SimpleForum.API.Models.Requests
{
    /// <summary>
    /// Object representing the request body to add a comment to a thread
    /// </summary>
    public class PostCommentRequest
    {
        public string Content { get; set; }
    }
}