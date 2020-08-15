namespace SimpleForum.Web.Models
{
    public class ResetPasswordViewModel : FormViewModel
    {
        public string Code { get; set; }
        public int UserID { get; set; }
    }
}