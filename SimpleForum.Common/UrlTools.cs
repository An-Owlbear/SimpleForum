using System.Linq;

namespace SimpleForum.Common
{
    public static class UrlTools
    {
        // Combines a list of URLs
        public static string UrlCombine(string[] paths)
        {
            return paths.Aggregate("", (acc , x) =>
            {
                return (acc.LastOrDefault(), x.FirstOrDefault()) switch
                {
                    ('/', '/') => $"{acc}{x.Substring(1)}",
                    ('/', _) => $"{acc}{x}",
                    (_, '/') => $"{acc}{x}",
                    _ => $"{acc}/{x}"
                };
            });
        }
    }
}