using System;

namespace SimpleForum.API.Models
{
    /// <summary>
    /// A thread object 
    /// </summary>
    public class Thread
    {
        public string Title;
        public int ID { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public User User { get; set;}

        public Thread(SimpleForum.Models.Thread thread)
        {
            Title = thread.Title;
            ID = thread.ThreadID;
            Content = thread.Content;
            DatePosted = thread.DatePosted;
            User = new User(thread.User);
        }
    }
}