using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleForum.Models
{
    public class Thread : IPost
    {
        [Key]
        public int ThreadID { get; set; }

        [NotMapped]
        public int ID
        {
            get => ThreadID;
            set => ThreadID = value;
        }
        
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public bool Pinned { get; set; }
        public bool Locked { get; set; }
        public string LockedBy { get; set; }
        public bool Deleted { get; set; }
        public string DeletedBy { get; set; }
        public string DeleteReason { get; set; }

        public int UserID { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}