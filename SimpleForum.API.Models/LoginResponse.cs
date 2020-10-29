using System;

namespace SimpleForum.API.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}