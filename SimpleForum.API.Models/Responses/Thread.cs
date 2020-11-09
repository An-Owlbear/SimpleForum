using System;

namespace SimpleForum.API.Models.Responses
{
    /// <summary>
    /// A thread object 
    /// </summary>
    public class Thread
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public bool Pinned { get; set; }
        public bool Locked { get; set; }
        public User User { get; set;}

        public Thread(SimpleForum.Models.Thread thread)
        {
            Title = thread.Title;
            ID = thread.ThreadID;
            Content = thread.Content;
            DatePosted = thread.DatePosted;
            Pinned = thread.Pinned;
            Locked = thread.Locked;
            User = new User(thread.User);
        }
    }
}