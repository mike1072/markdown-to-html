using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownToHtml
{
    public class Link
    {
        public string Text { get; }
        public string Url { get; }

        public Link(string text, string url)
        {
            Text = text;
            Url = url;
        }
    }
}
