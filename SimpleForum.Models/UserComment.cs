using System;

namespace SimpleForum.Models
{
    public class UserComment
    {
        public int UserCommentID { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        
        public int UserID { get; set; }
        public virtual User User { get; set; }
        
        public int UserPageID { get; set; }
        public virtual User UserPage { get; set; }
    }
}