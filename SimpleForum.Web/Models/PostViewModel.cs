using SimpleForum.Models;

namespace SimpleForum.Web.Models
{
    public class PostViewModel
    {
        public IPost Post { get; set; }
        public string ID { get; set; }
        public PostType PostType { get; set; }
        public User CurrentUser { get; set; }
    }

    public enum PostType
    {
        ThreadPost,
        Comment,
        UserComment
    }
}