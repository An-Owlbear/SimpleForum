using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleForum.Models
{
    public class Comment : IPost
    {
        [Key]
        public int CommentID { get; set; }
        
        [NotMapped]
        public int ID
        {
            get => CommentID;
            set => CommentID = value;
        }
        
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public bool Deleted { get; set; }
        public string DeletedBy { get; set; }
        public string DeleteReason { get; set; }

        public int UserID { get; set; }
        public virtual User User { get; set; }
        
        public int ThreadID { get; set; }
        public virtual Thread Thread { get; set; }
    }
}