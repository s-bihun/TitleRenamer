using System.Collections.Generic;

namespace TitleRename.IO {
    public interface IFileSystem {
        IEnumerable<IFile> AllFiles(string directory);
        void Copy(string sourceFile, string destinationFile, bool overwrite);
    }
}
