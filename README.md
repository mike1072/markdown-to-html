# markdown-to-html

This is a .NET Core 3.0 console application for generating HTML versions of Markdown files using [Markdig](https://github.com/lunet-io/markdig).

## Usage

`dotnet md2html.dll -f style/g3icon.ico -s https://gibberlings3.github.io/Documentation/readmes/style/g3md.css -o outputDir .`

This will locate all .md and .markdown files within the current directory and generate HTML versions of them in outputDir. The HTML files will have favicon and stylesheet tags set according to the -f and -s parameters.

## Publishing

Execute the following command within the MarkdownToHtml directory to pack the application and dependencies into a folder for deployment.

`dotnet publish -c Release`

## License

This project is released under the [MIT license](LICENSE).
