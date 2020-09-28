using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleForum.Internal
{
    public static class Tools
    {
        public static string GenerateCode(int length)
        {
            string chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int GetUserID(ClaimsPrincipal user)
        {
            Claim claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim.Value);
        }

        public static SimpleForumConfig GetConfig(string filename)
        {
            using StreamReader reader = new StreamReader(filename);
            string json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<SimpleForumConfig>(json);
        }
    }
}