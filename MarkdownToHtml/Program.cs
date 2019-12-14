using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace MarkdownToHtml
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        private static void Run(Options options)
        {
            var converter = new Converter(options.Language, options.FaviconPath, options.StylesheetPath);

            var fileNames = FileUtility.GetFilenamePairs(options.Input, options.OutputDirectory).ToArray();

            foreach (var (input, output) in fileNames)
            {
                converter.ConvertFile(input, output);
            }

            if (fileNames.Length == 0)
            {
                Console.WriteLine("Could not find any Markdown files to convert.");
            }
        }
    }
}
