using System.IO;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Workers.Test
{
    [TestFixture]
    public class ErrorReportReporterTests
    {
        [Test, Category(Categories.Unit)]
        public void DirectoryToMoveReportedReportsToIsCreated()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                const string subDirName = "reported";
                var subDir = Path.Combine(directory, subDirName);
                var errorReportConfiguration = new ErrorReportConfiguration(directory, subDir);
                var container = GetErrorReportReporter(errorReportConfiguration);
                var errorReportReporter = container.ErrorReportReporter;

                //When
                errorReportReporter.Start();

                //Then
                Directory.Exists(subDir).Should().BeTrue();
            });
        }

        [Test, Category(Categories.Unit)]
        public void ErrorReportReporterIsOnlyStartedOnce()
        {
            //Given
            var errorReportConfiguration =new ErrorReportConfiguration("./", "./");
            var container = GetErrorReportReporter(errorReportConfiguration);
            var fswMock = container.FileSystemWatcherMock;
            var fileSystemWatcherStartTimesCalled = 0;
            fswMock.Setup(fsw => fsw.Start(It.IsAny<string>())).Callback(() =>
            {
                fileSystemWatcherStartTimesCalled++;
            });
            var errorReportReporter = container.ErrorReportReporter;

            //When
            errorReportReporter.Start();
            errorReportReporter.Start();

            //Then
            fileSystemWatcherStartTimesCalled.Should().Be(1);
        }

        private static GetContainer GetErrorReportReporter(ErrorReportConfiguration errorReportConfiguration)
        {
            var fileSystemWatcherMock = new Mock<IFileSystemWatcher>();
            var gitHubIssueCreatorMock = new Mock<IGitHubIssueCreator>();
            var errorReportReporter = new ErrorReportReporter(fileSystemWatcherMock.Object, errorReportConfiguration, gitHubIssueCreatorMock.Object);
            return new GetContainer(fileSystemWatcherMock, errorReportReporter, errorReportConfiguration);
        }

        private class GetContainer
        {
            public Mock<IFileSystemWatcher> FileSystemWatcherMock { get; private set; }
            public ErrorReportReporter ErrorReportReporter { get; private set; }
            public ErrorReportConfiguration ErrorReportConfiguration { get; private set; }

            public GetContainer(Mock<IFileSystemWatcher> fileSystemWatcherMock, ErrorReportReporter errorReportReporter, ErrorReportConfiguration errorReportConfiguration)
            {
                FileSystemWatcherMock = fileSystemWatcherMock;
                ErrorReportReporter = errorReportReporter;
                ErrorReportConfiguration = errorReportConfiguration;
            }
        }
    }
}
