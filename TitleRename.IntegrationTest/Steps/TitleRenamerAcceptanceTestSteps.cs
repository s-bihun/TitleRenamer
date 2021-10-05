using System.Collections.Generic;
using System.Linq;
using Moq;
using TechTalk.SpecFlow;
using TitleRename.IO;
using Shouldly;

namespace TitleRename.IntegrationTest.Steps {
    [Binding]
    public class TitleRenamerAcceptanceTestSteps {
        private const string FileNameColumn = "FileName";
        private readonly ScenarioContext ScenarioContext;
        private Dictionary<string, List<Mock<IFile>>> InMemoryFileSystem = new Dictionary<string, List<Mock<IFile>>>();
        private Dictionary<string, string> InMemoryFileContents = new Dictionary<string, string>();
        private HashSet<string> ChangedFileList = new HashSet<string>();
        private readonly Mock<IFileSystem> FileSystemMock = new(MockBehavior.Strict);
        private XmlReplacer.ReplacementInfo Info;

        public TitleRenamerAcceptanceTestSteps(ScenarioContext scenarioContext) {
            ScenarioContext = scenarioContext;
            
            FileSystemMock.Setup(x => x.AllFiles(It.IsAny<string>())).Returns((string x) => InMemoryFileSystem[x].Select(y => y.Object));
            FileSystemMock.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Callback((string source, string dest, bool overwrite) => {
                    ChangedFileList.Add(dest);
                    IFile file = null;
                });
        }

        private static string GetExtension(string fullFileName) {
            return null;
        }

        [Given(@"directory ""(.*)"" contains files")]
        public void GivenDirectoryContainsFiles(string directory, Table fileNames) {
            var files = new List<Mock<IFile>>();
            foreach (string name in fileNames.Rows.Select(x => x[FileNameColumn])) {
                var fileMock = new Mock<IFile>(MockBehavior.Strict);
                fileMock.SetupGet(x => x.FullName).Returns(GetFullname(directory, name));
                fileMock.SetupGet(x => x.Extension).Returns(GetExtension(name));
                // setup streams
                files.Add(fileMock);
            }
            InMemoryFileSystem.Add(directory, files);
        }

        [Given(@"file ""(.*)"" contains")]
        public void GivenFileContains(string fullFileName, string givenContent) {
            ScenarioContext.Pending();
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
            ScenarioContext.Pending();
        }


        [Then(@"rest of the files should be unchanged")]
        public void ThenRestOfTheFilesShouldBeChanged(string p0) {
            ChangedFileList.Count.ShouldBe(0);
        }

        private static string GetFullname(string directory, string name)
        {
            return $"{directory}/{name}";
        }

        private static string GetPath(string fullFileName) {
            return null;
        }

        private static string GetName(string fullFileName) {
            return null;
        }
    }
}
