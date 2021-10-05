using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TitleRename.IO {
    internal sealed class FileSystem : IFileSystem {
        public IEnumerable<IFile> AllFiles(string directory) {
            var directoryInfo = new DirectoryInfo(directory);
            return directoryInfo.EnumerateFiles().Select(x => new File(x.FullName.Substring(0, x.FullName.Length - x.Extension.Length), x.Extension));
        }

        public void Copy(string sourceFileName, string destinationFileName, bool overwrite) {
            System.IO.File.Copy(sourceFileName, destinationFileName, overwrite);
        }
    }
}
