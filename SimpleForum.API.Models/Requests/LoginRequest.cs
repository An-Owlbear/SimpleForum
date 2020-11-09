namespace SimpleForum.API.Models.Requests
{
    /// <summary>
    /// Represents the request body used for logging in
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}