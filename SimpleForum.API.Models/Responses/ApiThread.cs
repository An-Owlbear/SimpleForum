using System;

namespace SimpleForum.API.Models.Responses
{
    /// <summary>
    /// A thread object 
    /// </summary>
    public class ApiThread : IApiPost
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public bool Pinned { get; set; }
        public bool Locked { get; set; }
        public int Replies { get; set; }
        public ApiUser User { get; set;}
        
        // Parameterless constructor for use with json deserialization
        public ApiThread() { }
        
        public ApiThread(SimpleForum.Models.Thread thread)
        {
            Title = thread.Title;
            ID = thread.ThreadID;
            Content = thread.Content;
            DatePosted = thread.DatePosted;
            Pinned = thread.Pinned;
            Locked = thread.Locked;
            Replies = thread.Comments?.Count ?? 0;
            User = new ApiUser(thread.User);
        }
    }
}