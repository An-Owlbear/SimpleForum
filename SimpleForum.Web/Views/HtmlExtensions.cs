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
        public static HtmlString RenderOutput(this IHtmlHelper htmlHelper, string message)
        {
            string addNewLines = string.Join(
                "<br/>",
                message.Split(new [] { Environment.NewLine }, StringSplitOptions.None)
                    .Select(HttpUtility.HtmlEncode));

            string output = Regex.Replace(addNewLines, @"\[[^\[\]\n\r]+\]", delegate(Match match)
            {
                string matchString = match.ToString().Replace("[", "").Replace("]", "");
                return $"<a href=\"{matchString}\">{matchString}</a>";
            });
            
            return new HtmlString(output);
        }
    }
}