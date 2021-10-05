using System.IO;

namespace TitleRename.IO {
    public interface IFile {
        string FullName { get; }
        string Extension { get; }
        TextReader CreateReader();
        TextWriter CreateWriter();
    }
}
