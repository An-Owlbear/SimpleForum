using System;

namespace SimpleForum.Models
{
    public class EmailCode
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime ValidUntil { get; set; }

        public int UserID { get; set; }
        public virtual User User { get; set; }
    }
}
