using System;
using System.Linq;
using System.Security.Claims;

namespace SimpleForum.Common
{
    public static class Tools
    {
        // Generates a random code of letters and numbers of the given length
        public static string GenerateCode(int length)
        {
            string chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Retrieves a user id from the give ClaimsPrincipal
        public static int GetUserID(ClaimsPrincipal user)
        {
            Claim claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim.Value);
        }
    }
}