using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleForum.Models
{
    using BCrypt.Net;
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        private string password;
        public string Password
        {
            get => password;
            set => password = BCrypt.HashPassword(value);
        }
        
        public DateTime SignupDate { get; set; }
        public string Bio { get; set; }
        
        public bool Activated { get; set; } = false;
        public string Role { get; set; } = "User";
        public bool CommentsLocked { get; set; }
        
        public bool Muted { get; set;}
        public string MuteReason { get; set; }
        public bool Banned { get; set; }
        public string BanReason { get; set; }
        public bool Deleted { get; set; }

        public int? ServerID { get; set; }
        public virtual IncomingServerToken Server { get; set; }
        public virtual ICollection<Thread> Threads { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        
        [InverseProperty("User")]
        public virtual ICollection<UserComment> UserComments { get; set; }
        [InverseProperty("UserPage")]
        public virtual ICollection<UserComment> UserPageComments { get; set; }
        
        public virtual ICollection<Notification> Notifications { get; set; }
        
        [NotMapped]
        public string FullUsername =>
            Server switch
            {
                null => Username,
                _ => $"{Username}@{Server.ShortAddress}"
            };

        public bool CheckPassword(string password) => BCrypt.Verify(password, Password);
    }
}