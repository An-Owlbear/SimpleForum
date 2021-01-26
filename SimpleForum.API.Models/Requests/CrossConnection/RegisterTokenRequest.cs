namespace SimpleForum.API.Models.Requests.CrossConnection
{
    public class RegisterTokenRequest
    {
        public string Token { get; set; }
        public string Domain { get; set; }
    }
}