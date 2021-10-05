using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TitleRename.IO;
using File = TitleRename.IO.File;

namespace TitleRename {
    public sealed class XmlReplacer {
        private const char ExtensionPrefix = '.';
        private IFileSystem FileSystem;
        private string BackupExtension = ".bak";

        public bool EnableReplacementsInContent = true;
        public HashSet<string> ExtensionsToProcess { get; private set; } = new HashSet<string>() { ".xml", ".xsl", ".xslt" };
        public HashSet<string> AttributesToProcess { get; private set; } = new HashSet<string> { "title" };
        public bool EnableBackups { get; set; } = true;
        public string BackupFileExtension {
            get {
                return BackupExtension;
            }
            set {
                if (value.Length == 0) { throw new ArgumentException("Extension should be a not empty string."); }

                BackupExtension = value;
                if (value[0] != ExtensionPrefix) {
                    BackupExtension = $"{ExtensionPrefix}{value}";
                }
            }
        }

        public XmlReplacer(IFileSystem fileSystem) {
            FileSystem = fileSystem;
        }

        public void SetExtensionsToProcess(HashSet<string> extensions) {
            ExtensionsToProcess = extensions;
        }

        public void SetAttributesToProcess(HashSet<string> attributes) {
            AttributesToProcess = attributes;
        }

        public ReplacementInfo ReplaceAllInDirectory(string replacePattern, string replacement, string directory) {
            var replacementInfo = new ReplacementInfo();
            foreach (IFile file in FileSystem.AllFiles(directory).Where(IsFileToProcess)) {
                try
                {
                    XDocument doc;
                    using (TextReader reader = file.CreateReader()) {
                        doc = XDocument.Load(reader);
                    }
                    int replacedCount = ReplaceRecursive(doc.Root, replacePattern, replacement);
                    if (replacedCount > 0) {
                        IFile backupFile = null;
                        if (EnableBackups) {
                            backupFile = new File(file.FullName, BackupExtension);
                            FileSystem.Copy(file.FullName, backupFile.FullName, true);
                        }
                        using (TextWriter writer = file.CreateWriter()) {
                            doc.Save(writer);
                        }
                        replacementInfo.AddSuccessfullyProcessedLog(file, backupFile, replacedCount);
                    }
                    replacementInfo.AddNothingToReplaceLog(file);
                }
                catch (Exception ex) {
                    replacementInfo.AddExceptionWhileProcessingLog(file, ex);
                }
            }
            return replacementInfo;
        }

        private bool IsFileToProcess(IFile file) {
            return !ExtensionsToProcess.Any() || ExtensionsToProcess.Contains(file.Extension) ||
                ExtensionsToProcess.Any(x => x.Length > 0 && x[0] != ExtensionPrefix && $"{ExtensionPrefix}{x}" == file.Extension);
        }

        private int ReplaceRecursive(XElement element, string replacePattern, string replacement) {
            int replacementsCount = 0;

            if (EnableReplacementsInContent) {
                foreach (XText textNode in element.Nodes().OfType<XText>()) {
                    int valuesToReplaceCount = Regex.Match(textNode.Value, replacePattern).Captures.Count;
                    if (valuesToReplaceCount > 0) {
                        textNode.Value = Regex.Replace(textNode.Value, replacePattern, replacement);
                        replacementsCount += valuesToReplaceCount;
                    }
                }
            }

            foreach (XAttribute attribute in element.Attributes().Where(x => AttributesToProcess.Contains(x.Name.LocalName))) {
                int attributeValuesToReplaceCount = Regex.Match(attribute.Value, replacePattern).Captures.Count;
                if (attributeValuesToReplaceCount > 0) {
                    attribute.Value = Regex.Replace(attribute.Value, replacePattern, replacement);
                    replacementsCount += attributeValuesToReplaceCount;
                }
            }

            foreach (XElement e in element.Elements()) {
                replacementsCount += ReplaceRecursive(e, replacePattern, replacement);
            }

            return replacementsCount;
        }

        public sealed class ReplacementInfo {
            public IList<string> SuccessfullyProcessedLogs { get; } = new List<string>();
            public IList<string> NothingToReplaceLogs { get; } = new List<string>();
            public IList<string> ExceptionWhileProcessingLogs { get; } = new List<string>();

            public void AddSuccessfullyProcessedLog(IFile file, IFile backup, int replacementCount) {
                string message = $"File \"{file.FullName}\" successfully processed, {replacementCount} entries was replaced." +
                    backup == null ? " No backup was created" : $" Previous version of the file is backed up to \"{backup.FullName}\".";
                SuccessfullyProcessedLogs.Add(message);
            }

            public void AddNothingToReplaceLog(IFile file) {
                string message = $"File \"{file.FullName}\" contain no entries to replace.";
                NothingToReplaceLogs.Add(message);
            }

            public void AddExceptionWhileProcessingLog(IFile file, Exception exception) {
                string message = $"File \"{file.FullName}\" can't be processed because of an exception: {exception.Message}";
                NothingToReplaceLogs.Add(message);
            }
        }
    }
}
