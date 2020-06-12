using System;
using System.Collections.Generic;

namespace SimpleForum.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime SignupDate { get; set; }
        
        public virtual ICollection<Thread> Threads { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}