using SimpleForum.Models;

namespace SimpleForum.API.Models.Responses.CrossConnection
{
    /// <summary>
    /// Represents user received when authenticated remote tokens
    /// </summary>
    public class CrossConnectionUser : ApiUser
    {
        public string Email { get; set; }

        public CrossConnectionUser(User user) : base(user)
        {
            Email = user.Email;
        }
        
        public CrossConnectionUser() { }
    }
}