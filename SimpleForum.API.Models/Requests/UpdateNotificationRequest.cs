namespace SimpleForum.API.Models.Requests
{
    /// <summary>
    /// Represents the request a user uses to update a notification
    /// </summary>
    public class UpdateNotificationRequest
    {
        public bool Read { get; set; }
    }
}