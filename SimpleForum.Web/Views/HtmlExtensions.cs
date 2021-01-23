using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using SimpleForum.TextParser;

namespace SimpleForum.Web.Views
{
    public static class HtmlExtensions
    {
        // Parsers a markdown string to html
        public static HtmlString ParseText(string text)
        {
            IEnumerable<MarkdownParser.MarkdownValue> markdown = MarkdownParser.ParseMarkdown(text);
            string result = MarkdownParser.MarkdownToHTML(markdown);
            return new HtmlString(result);
        } 
    }
}