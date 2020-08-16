using System.Collections.Generic;
using SimpleForum.Models;

namespace SimpleForum.Web.Models
{
    public class UserPageViewModel
    {
        public User User { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        public User CurrentUser { get; set; }
        public IEnumerable<UserComment> CurrentPageComments { get; set; }
    }
}