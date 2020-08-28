using System.Collections.Generic;
using SimpleForum.Models;

namespace SimpleForum.Web.Models
{
    public class ThreadViewModel
    {
        public Thread Thread { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        public User User { get; set; }
    }
}