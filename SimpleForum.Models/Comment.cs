using System;

namespace SimpleForum.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        
        public int UserID { get; set; }
        public virtual User User { get; set; }
        
        public int ThreadID { get; set; }
        public virtual Thread Thread { get; set; }
    }
}