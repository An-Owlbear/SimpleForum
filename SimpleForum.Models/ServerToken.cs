using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleForum.Models
{
    /// <summary>
    /// Represents a token used by servers for cross site interactions
    /// </summary>
    public abstract class ServerToken
    {
        public string Address { get; set; }
        public string ApiAddress { get; set; }
        public string CrossConnectionAddress { get; set; }
        public string Token { get; set; }

        [NotMapped]
        public string ShortAddress
        {
            get => Address.Replace("https://", "").Replace("http://", "");
        }
    }

    public class OutgoingServerToken : ServerToken
    {
        [Key]
        public int OutgoingServerTokenID { get; set; }
    }

    public class IncomingServerToken : ServerToken
    {
        [Key]
        public int IncomingServerTokenID { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}