using System;
using SimpleForum.Models;

namespace SimpleForum.API.Models.Responses
{
    /// <summary>
    /// A notification object
    /// </summary>
    public class ApiNotification
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Read { get; set; }

        /// <summary>
        /// Creates an API model from internal notification model
        /// </summary>
        /// <param name="notification">The internal model to use</param>
        public ApiNotification(Notification notification)
        {
            ID = notification.NotificationID;
            Title = notification.Title;
            Content = notification.Content;
            DateCreated = notification.DateCreated;
            Read = notification.Read;
        }
    }
}