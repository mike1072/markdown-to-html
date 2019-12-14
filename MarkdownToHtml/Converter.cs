using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace MarkdownToHtml
{
    public class Converter
    {
        private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder().UseAutoIdentifiers(AutoIdentifierOptions.GitHub).Build();

        private readonly string _language;
        private readonly string _favicon;
        private readonly string _stylesheet;

        public Converter(string language, string favicon, string stylesheet)
        {
            _language = language;
            _favicon = favicon;
            _stylesheet = stylesheet;
        }

        public void ConvertFile(string inputFilePath, string outputFilePath)
        {
            try
            {
                var markdown = File.ReadAllText(inputFilePath);
                using var writer = new StreamWriter(outputFilePath);

                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);

                WriteHtml(markdown, writer, fileName);
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine($"Encountered I/O error when attempting to convert {inputFilePath} to {outputFilePath}:");
                Console.Error.WriteLine(ex);
            }
        }

        public void TestConvertMarkdown(string markdown)
        {
            WriteHtml(markdown, Console.Out, "Readme");
        }

        private void WriteHtml(string markdown, TextWriter writer, string fileName)
        {
            var document = Markdown.Parse(markdown, _pipeline);
            var title = GetFirstHeader(document) ?? fileName;
            var menuLinks = GetMenuLinks(document);

            // make adjustments to the markdown document
            FixMarkdownLinks(document);

            // output doctype and opening html tag
            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine($@"<html lang=""{_language}"">");

            // output head tag
            WriteHeadTag(writer, title);

            // output body
            writer.WriteLine("<body>");
            WriteBodyHeaderTag(writer, menuLinks);
            WriteBodyMainTag(writer, document);
            writer.WriteLine("</body>");

            // output closing html tag
            writer.WriteLine("</html>");

            writer.Flush();
        }

        private void WriteHeadTag(TextWriter writer, string title)
        {
            writer.WriteLine("<head>");
            writer.WriteLine(@"<meta charset=""utf-8"">");
            writer.WriteLine(@"<meta name=""viewport"" content=""width=device-width, initial-scale=1"">");
            writer.WriteLine($"<title>{title}</title>");
            if (_favicon != null)
            {
                writer.WriteLine($@"<link href=""{_favicon}"" rel=""icon"" type=""image/bmp"" />");
            }
            if (_stylesheet != null)
            {
                writer.WriteLine($@"<link href=""{_stylesheet}"" rel=""stylesheet"" type=""text/css"" />");
            }
            writer.WriteLine("</head>");
        }

        private void WriteBodyMainTag(TextWriter writer, MarkdownDocument document)
        {
            writer.WriteLine(@"<main class=""content"">");
            var renderer = new HtmlRenderer(writer);
            _pipeline.Setup(renderer);
            renderer.Render(document);
            writer.WriteLine("</main>");
        }

        private static void WriteBodyHeaderTag(TextWriter writer, IEnumerable<Link> menuLinks)
        {
            writer.WriteLine("<header>");
            writer.WriteLine("<ul>");

            foreach (var link in menuLinks)
            {
                writer.WriteLine($@"<li><a href=""{link.Url}"">{link.Text}</a>");
            }

            writer.WriteLine("</ul>");
            writer.WriteLine("</header>");
        }

        private static void FixMarkdownLinks(MarkdownDocument document)
        {
            // fix relative links to other markdown documents that are being converted
            foreach (var link in document.Descendants<ParagraphBlock>().SelectMany(b => b.Inline.Descendants<LinkInline>()).Where(l => !l.IsImage))
            {
                if (!link.Url.Contains("/") && (link.Url.ToLower().EndsWith(".md") || link.Url.ToLower().EndsWith(".markdown")))
                {
                    link.Url = Path.ChangeExtension(link.Url, "html").ToLower();
                }
            }
        }

        private static string GetFirstHeader(ContainerBlock document)
        {
            return document.Descendants<HeadingBlock>().FirstOrDefault(h => h.Level == 1)?.Inline?.FirstChild.ToString();
        }

        private static IEnumerable<Link> GetMenuLinks(ContainerBlock document)
        {
            var headers = document.Descendants<HeadingBlock>()
                .Where(h => h.Level == 2)
                .Select(h => h.Inline?.FirstChild.ToString())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(GenerateLink)
                .ToArray();

            return headers.Length < 2 ? Enumerable.Empty<Link>() : headers;

            static Link GenerateLink(string headingText) => new Link(headingText, "#" + LinkHelper.UrilizeAsGfm(headingText));
        }
    }
}
