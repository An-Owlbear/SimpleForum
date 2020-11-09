namespace SimpleForum.API.Models.Requests
{
    /// <summary>
    /// Object representing the request body to create a new thread
    /// </summary>
    public class CreateThreadRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}