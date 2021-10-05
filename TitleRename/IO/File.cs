using System.IO;

namespace TitleRename.IO {
    internal sealed class File : IFile {
        public string FullName { get; }
        public string Extension { get; }

        public File(string fullNameWithoutExtension, string extension) {
            Extension = extension;
            FullName = fullNameWithoutExtension + extension;
        }

        public TextReader CreateReader() {
            return System.IO.File.OpenText(FullName);
        }

        public TextWriter CreateWriter() {
            return System.IO.File.CreateText(FullName);
        }
    }
}
