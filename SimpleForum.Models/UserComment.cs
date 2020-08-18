using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleForum.Models
{
    public class UserComment : IPost
    {
        [Key]
        public int UserCommentID { get; set; }

        [NotMapped]
        public int ID
        {
            get => UserCommentID;
            set => UserCommentID = value;
        }
        
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public bool Deleted { get; set; }
        public string DeletedBy { get; set; }
        public string DeleteReason { get; set; }

        public int UserID { get; set; }
        public virtual User User { get; set; }

        public int UserPageID { get; set; }
        public virtual User UserPage { get; set; }
    }
}