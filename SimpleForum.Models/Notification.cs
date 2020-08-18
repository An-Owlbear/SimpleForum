using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleForum.Models
{
    /// <summary>
    /// Class representing a notification
    /// </summary>
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }
        
        /// <summary>
        /// The title of the notification
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// The content of the notification
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// The date the notification was created
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}