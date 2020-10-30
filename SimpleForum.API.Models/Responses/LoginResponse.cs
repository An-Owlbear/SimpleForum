using System;

namespace SimpleForum.API.Models.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}