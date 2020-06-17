namespace SimpleForum.Models
{
    public class AuthToken
    {
        public int AuthTokenID { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int Time { get; set;}
        
        public int UserID { get; set; }
        public virtual User User { get; set; }
    }
}