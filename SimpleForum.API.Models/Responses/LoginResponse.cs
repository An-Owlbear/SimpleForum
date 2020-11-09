using System;

namespace SimpleForum.API.Models.Responses
{
    /// <summary>
    /// Object representing the response to a login request
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}