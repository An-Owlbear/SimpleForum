using System.ComponentModel.DataAnnotations;

namespace SimpleForum.Models
{
    /// <summary>
    /// Represents a token used by servers for cross site interactions
    /// </summary>
    public abstract class ServerToken
    {
        [Key]
        public string Address { get; set; }
        public string Token { get; set; }
    }
    
    public class OutgoingServerToken : ServerToken { }
    public class IncomingServerToken : ServerToken { }
}