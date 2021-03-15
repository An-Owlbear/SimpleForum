namespace SimpleForum.Web.Models
{
    // Represents the information of a button for a MessageViewModel
    public class ButtonModel
    {
        public string Text { get; set; }
        public string Method { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}