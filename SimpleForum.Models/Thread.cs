using System;
using System.Collections.Generic;

namespace SimpleForum.Models
{
    public class Thread
    {
        public int ThreadID { get; set; }
        public string Title { get; set; }
        public DateTime DatePosted { get; set; }
        
        public int UserID { get; set; }
        public virtual User User { get; set; }
        
        public virtual ICollection<Comment> Comments { get; set; }
    }
}