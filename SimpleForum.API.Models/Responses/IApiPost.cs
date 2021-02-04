using System;

namespace SimpleForum.API.Models.Responses
{
    public interface IApiPost
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public ApiUser User { get; set; }
        public DateTime DatePosted { get; set; }
    }
}