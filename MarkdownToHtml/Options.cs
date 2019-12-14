using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace MarkdownToHtml
{
    public class Options
    {
        [Value(0, MetaName = "input", Required = true, HelpText = "A list of Markdown files or directories containing Markdown files to convert.")]
        public IEnumerable<string> Input { get; set; }

        [Option('o', "output", Required = false, HelpText = "Directory to store converted HTML files.")]
        public string OutputDirectory { get; set; }

        [Option('f', "favicon", Required = false, HelpText = "Path to favicon to use for the converted HTML files.")]
        public string FaviconPath { get; set; }

        [Option('s', "stylesheet", Required = false, HelpText = "Path to stylesheet to use for the converted HTML files.")]
        public string StylesheetPath { get; set; }

        [Option('l', "language", Default = "en", Required = false, HelpText = "ISO language code to use for the converted HTML files.")]
        public string Language { get; set; }

        [Usage(ApplicationAlias = "mdtohtml")]
        public static IEnumerable<Example> Examples =>
            new List<Example>
            {
                new Example("Generate an HTML version of the files 'README.md' and 'CHANGELOG.md' and place them in the 'output' directory",
                    new Options
                    {
                        Input = new List<string> { "README.md", "CHANGELOG.md" },
                        OutputDirectory = "output",
                        FaviconPath = "style/g3icon.ico",
                        StylesheetPath = "https://gibberlings3.github.io/Documentation/readmes/style/g3md.css"
                    }),
            };
    }
}
