using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SimpleForum.Web.Views
{
    public static class HtmlExtensions
    {
        public static HtmlString RenderOutput(string message)
        {
            string addNewLines = string.Join(
                "<br/>",
                message.Split(new [] { Environment.NewLine }, StringSplitOptions.None)
                    .Select(HttpUtility.HtmlEncode));

            string addLinks = Regex.Replace(addNewLines, @"\[[^\[\]\n\r]+\]\([^\(\)\n\r]+\)", delegate(Match match)
            {
                string matchString = match.ToString();
                string title = Regex.Match(matchString, @"\[[^\[\]\n\r]+\]").Value
                    .Replace("[", "").Replace("]","");
                string url = Regex.Match(matchString, @"\([^\(\)\n\r]+\)").Value
                    .Replace("(","").Replace(")","");
                return $"<a class=\"markdown-link\" href=\"{url}\">{title}</a>";
            });
            
            string output = Regex.Replace(addLinks, @"\[[^\[\]\n\r]+\]", delegate(Match match)
            {
                string matchString = match.ToString().Replace("[", "").Replace("]", "");
                return $"<a class=\"markdown-link\" href=\"{matchString}\">{matchString}</a>";
            });
            
            return new HtmlString(output);
        }

        public static HtmlString RenderOutput(this IHtmlHelper helper, string message)
        {
            return RenderOutput(message);
        }
    }
}