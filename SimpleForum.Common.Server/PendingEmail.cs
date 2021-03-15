namespace SimpleForum.Common.Server
{
    // Represents an email to be sent when SaveChanges is called
    public class PendingEmail
    {
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsHTML { get; set; }
    }
}