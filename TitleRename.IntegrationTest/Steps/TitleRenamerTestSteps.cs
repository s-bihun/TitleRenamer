using System.Collections.Generic;
using System.Linq;
using Moq;
using TechTalk.SpecFlow;
using TitleRename.IO;
using Shouldly;
using System.Text;
using System.IO;

namespace TitleRename.IntegrationTest.Steps {
    [Binding]
    public class TitleRenamerTestSteps {
        private const string FileNameColumn = "FileName";
        private readonly Mock<IFileSystem> FileSystemMock = new(MockBehavior.Strict);
        private XmlReplacer.ReplacementInfo Info;

        public TitleRenamerTestSteps() {
            FileSystemMock.Setup(x => x.AllFiles(It.IsAny<string>())).Returns((string d) => GetAllFiles(d));
            FileSystemMock.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Callback((string s, string d, bool o) => CopyFile(s, d, o));
        }

        [Given(@"directory ""(.*)"" contains files")]
        public void GivenDirectoryContainsFiles(string directory, Table fileNames) {
            var files = new List<Mock<IFile>>();
            foreach (string name in fileNames.Rows.Select(x => x[FileNameColumn])) {
                files.Add(CreateFile(directory, name));
            }
            InMemoryFileSystem.Add(directory, files);
        }

        [Given(@"file ""(.*)"" contains")]
        public void GivenFileContains(string fullFileName, string givenContent) {
            UpdateFileContent(fullFileName, givenContent);
        }

        [When(@"""(.*)"" is replaced by ""(.*)"" in directory ""(.*)""")]
        public void WhenIsReplacedByInDirectory(string replacePattern, string replacement, string directory) {
            var xmlReplacer = new XmlReplacer(FileSystemMock.Object);
            Info = xmlReplacer.ReplaceAllInDirectory(replacePattern, replacement, directory);
        }

        [Then(@"directory ""(.*)"" should contain")]
        public void ThenDirectoryShouldContain(string directory, Table expectedFileNames) {
            IList<Mock<IFile>> resultFiles = InMemoryFileSystem[directory];
            resultFiles.Count.ShouldBe(expectedFileNames.RowCount);
            resultFiles.Select(x => x.Object.FullName).ShouldBe(expectedFileNames.Rows.Select(x => GetFullname(directory, x[FileNameColumn])), true);
        }

        [Then(@"file ""(.*)"" should contain")]
        public void ThenFileShouldContain(string fullFileName, string expectedContent) {
            ChangedFileList.Remove(fullFileName);
            InMemoryFileContents[fullFileName].ToString().ShouldBe(expectedContent);
        }


        [Then(@"rest of the files should be unchanged")]
        public void ThenRestOfTheFilesShouldBeChanged() {
            ChangedFileList.Count.ShouldBe(0);
        }

        private Dictionary<string, List<Mock<IFile>>> InMemoryFileSystem = new Dictionary<string, List<Mock<IFile>>>();
        private Dictionary<string, StringBuilder> InMemoryFileContents = new Dictionary<string, StringBuilder>();
        private HashSet<string> ChangedFileList = new HashSet<string>();

        private static string GetFullname(string directory, string name)
        {
            return $"{directory}/{name}";
        }

        private static string GetDirectory(string fullFileName) {
            int directoryLength = fullFileName.LastIndexOf('/');
            return fullFileName.Substring(0, directoryLength);
        }

        private static string GetName(string fullFileName) {
            int pathLength = fullFileName.LastIndexOf('/');
            return fullFileName.Substring(pathLength + 1);
        }

        private static string GetExtension(string fileName) {
            int pathLength = fileName.LastIndexOf('/');
            int dotIndex = fileName.LastIndexOf('.');
            if (dotIndex <= pathLength) {
                return "";
            } else {
                return fileName.Substring(dotIndex, fileName.Length - dotIndex);
            }
        }

        private void CopyFile(string source, string destination, bool overwrite) {
            string sourseDirectory = GetDirectory(source);
            string sourseName = GetName(source);
            string destDirectory = GetDirectory(destination);
            string destName = GetName(destination);

            if (overwrite || !InMemoryFileSystem[destDirectory].Any(x => x.Object.FullName == destName)) {
                InMemoryFileContents[destination] = new StringBuilder(InMemoryFileContents[source].ToString());
                if (!InMemoryFileSystem[destDirectory].Any(x => x.Object.FullName == destination)) {
                    InMemoryFileSystem[destDirectory].Add(CreateFile(destDirectory, destName));
                }
            }
            ChangedFileList.Add(destination);
        }

        private IEnumerable<IFile> GetAllFiles(string directory) {
            return InMemoryFileSystem[directory].Select(y => y.Object);
        }

        private Mock<IFile> CreateFile(string directory, string name) {
            var fileMock = new Mock<IFile>(MockBehavior.Strict);
            fileMock.SetupGet(x => x.FullName).Returns(GetFullname(directory, name));
            fileMock.SetupGet(x => x.Extension).Returns(GetExtension(name));
            fileMock.Setup(x => x.CreateReader()).Returns(() => new StringReader(InMemoryFileContents[GetFullname(directory, name)].ToString()));
            fileMock.Setup(x => x.CreateWriter()).Returns(() => {
                ChangedFileList.Add(GetFullname(directory, name));
                InMemoryFileContents[GetFullname(directory, name)].Clear();
                return new StringWriter(InMemoryFileContents[GetFullname(directory, name)]);
            });
            return fileMock;
        }

        private void UpdateFileContent(string fullName, string content) {
            InMemoryFileContents[fullName] = new StringBuilder(content);
        }
    }
}
