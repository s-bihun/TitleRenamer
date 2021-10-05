namespace TitleRename.IO {
    public sealed class File : IFile {
        public string FullName { get; private set; }
        public string Extension { get; private set; }

        public File(string fullNameWithoutExtension, string extension) {
            Extension = extension;
            FullName = fullNameWithoutExtension + extension;
        }
    }
}
