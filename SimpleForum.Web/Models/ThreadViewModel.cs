using System.Collections.Generic;
using SimpleForum.Models;

namespace SimpleForum.Web.Models
{
    public class ThreadViewModel
    {
        public string Title { get; set; }
        public int ThreadID { get; set; }
        public bool Pinned { get; set; }
        public bool Locked { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        public User User { get; set; }
    }
}