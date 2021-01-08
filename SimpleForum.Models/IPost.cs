using System;

namespace SimpleForum.Models
{
    public interface IPost
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTime DatePosted { get; set; }
        public string Content { get; set; }
        public bool Deleted { get; set; }
        public string DeletedBy { get; set; }
        public string DeleteReason { get; set; }

        public User User { get; set; }
    }
}