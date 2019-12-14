using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownToHtml
{
    public static class FileUtility
    {
        public static IEnumerable<(string input, string output)> GetFilenamePairs(IEnumerable<string> inputs, string outputDirectoryName)
        {
            try
            {
                var inputFiles = GetMarkDownFilesFromInput(inputs);
                var outputDirectory = outputDirectoryName == null ? null : new DirectoryInfo(outputDirectoryName);

                return inputFiles.Select(f => (f.FullName, GenerateOutputFilename(f, outputDirectory ?? f.Directory)));
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Could not access requested files or directories.");
                throw;
            }
        }

        private static IEnumerable<FileInfo> GetMarkDownFilesFromInput(IEnumerable<string> inputs)
        {
            var files = new List<FileInfo>();
            foreach (string input in inputs)
            {
                if (Directory.Exists(input))
                {
                    files.AddRange(GetMarkDownFilesInDirectory(input));
                }
                else if (File.Exists(input) && (input.EndsWith(".md") || input.EndsWith(".markdown")))
                {
                    files.Append(new FileInfo(input));
                }
            }

            return files
                .GroupBy(f => f.Name)
                .Select(g => g.First())
                .OrderBy(f => f.Name);
        }

        private static IEnumerable<FileInfo> GetMarkDownFilesInDirectory(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);

            return directoryInfo
                .GetFiles("*.md")
                .Concat(directoryInfo.GetFiles("*.markdown"));
        }

        private static string GenerateOutputFilename(FileInfo inputFile, DirectoryInfo outputDirectory)
        {
            return Path.Join(outputDirectory.FullName, Path.ChangeExtension(inputFile.Name, "html").ToLower());
        }
    }
}
