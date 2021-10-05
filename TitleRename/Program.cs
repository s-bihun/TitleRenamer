using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace TitleRename {
    static class Program {
        private static readonly IList<string> XmlExtensions = new List<string> { ".xml", ".xsl", ".xslt" };
        private const string ReplacePattern = "(?<!SDL )Trisoft";
        private const string Replacement = "SDL Trisoft";


        private static void Main() {
            var directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
            var settings = new XmlReaderSettings();
            settings.Async = true;
            foreach (FileInfo file in directoryInfo.EnumerateFiles()) {
                if (!XmlExtensions.Contains(file.Extension)) { continue; }

                try {
                    var doc = XDocument.Load(file.FullName);
                    if (TryReplaceTitleRecursive(doc.Root)) {
                        File.Copy(file.FullName, $"{file.FullName}.bak", true);
                        doc.Save($"{file.FullName}");
                        Console.WriteLine($"Titles in file {file.FullName} are replaced.");
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"Can't replace titles in file {file.FullName} because of the exception.");
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }

        private static bool TryReplaceTitleRecursive(XElement element) {
            bool wasReplaced = false;
            string value = element.Value;
            if (Regex.IsMatch(value, ReplacePattern)) {
                element.Value = Regex.Replace(value, ReplacePattern, Replacement);
                wasReplaced = true;
            }
            var title = element.Attribute("title");
            if (title != null && Regex.IsMatch(title.Value, ReplacePattern)) {
                title.Value = Regex.Replace(title.Value, ReplacePattern, Replacement);
                wasReplaced = true;
            }

            foreach (XElement e in element.Elements()) {
                wasReplaced |= TryReplaceTitleRecursive(e);
            }
            return wasReplaced;
        }
    }
}
