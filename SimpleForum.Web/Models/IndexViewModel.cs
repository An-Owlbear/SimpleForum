using System.Collections.Generic;
using SimpleForum.Models;

namespace SimpleForum.Web.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Thread> Threads { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        public bool EmailVerified;
        public bool EmailVerificationRequired { get; set; }
    }
}