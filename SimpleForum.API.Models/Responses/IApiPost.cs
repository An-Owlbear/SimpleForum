using System;

namespace SimpleForum.API.Models.Responses
{
    /// <summary>
    /// Represents any type of api post
    /// </summary>
    public interface IApiPost
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public ApiUser User { get; set; }
        public DateTime DatePosted { get; set; }
    }
}