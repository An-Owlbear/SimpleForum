using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleForum.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime SignupDate { get; set; }
        public bool Activated { get; set; } = false;
        public string Role { get; set; } = "User";
        public bool CommentsLocked { get; set; }

        public virtual ICollection<Thread> Threads { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        
        [InverseProperty("User")]
        public virtual ICollection<UserComment> UserComments { get; set; }
        [InverseProperty("UserPage")]
        public virtual ICollection<UserComment> UserPageComments { get; set; }
    }
}