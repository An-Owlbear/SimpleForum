using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleForum.Models
{
    public class AuthToken
    {
        [Key]
        public string Token { get; set; }
        public DateTime ValidUntil { get; set;}
        
        public int UserID { get; set; }
        public virtual User User { get; set; }
    }
}