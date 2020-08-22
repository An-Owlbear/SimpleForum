using System.Collections.Generic;
using SimpleForum.Models;

namespace SimpleForum.Web.Models
{
    public class NotificationsViewModel
    {
        public IEnumerable<Notification> Notifications { get; set; }
    }
}