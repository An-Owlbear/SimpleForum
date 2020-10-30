using System;

namespace SimpleForum.API.Models.Responses
{
    /// <summary>
    /// A user object
    /// </summary>
    public class User
    {
        public string Username { get; set; }
        public int ID { get; set; }
        public int Posts { get; set; }
        public int Comments { get; set; }
        public bool Banned { get; set; }
        public DateTime DateJoined { get; set; }

        /// <summary>
        /// Creates an API model user from a database model user
        /// </summary>
        /// <param name="user">The database model user</param>
        public User(SimpleForum.Models.User user)
        {
            Username = user.Username;
            ID = user.UserID;
            Posts = user.Threads.Count;
            Comments = user.Comments.Count + user.UserComments.Count;
            Banned = Banned;
            DateJoined = user.SignupDate;
        }
    }
}