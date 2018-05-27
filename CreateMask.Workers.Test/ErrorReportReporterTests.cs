using System.IO;
using System.Threading;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
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
        public void IssueIsCreatedFromAlreadyExistingErrorReport()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                var errorReport = new ErrorReport();
                var processedFolder = Path.Combine(directory, "processed");
                var json = JsonConvert.SerializeObject(errorReport);
                const string errorReportName = "error-report.json";
                var filePath = Path.Combine(directory, errorReportName);
                File.WriteAllText(filePath, json);
                var config = new ErrorReportConfiguration(directory, processedFolder);
                var container = GetErrorReportReporter(config);
                var errorReportReporter = container.ErrorReportReporter;
                var githubIssueCreatorMock = container.GitHubIssueCreatorMock;
                githubIssueCreatorMock.Setup(gic => gic.CreateIssue(It.IsAny<ErrorReport>()));

                //When
                errorReportReporter.Start();

                //Then
                githubIssueCreatorMock.Verify(gic => gic.CreateIssue(It.IsAny<ErrorReport>()), Times.Once);
            });
        }

        [Test, Category(Categories.Unit)]
        public void AlreadyExistingErrorReportIsMovedToProcessedFolder()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                var errorReport = new ErrorReport();
                var processedFolder = Path.Combine(directory, "processed");
                var json = JsonConvert.SerializeObject(errorReport);
                const string errorReportName = "error-report.json";
                var filePath = Path.Combine(directory, errorReportName);
                var expectedFilePath = Path.Combine(processedFolder, errorReportName);
                File.WriteAllText(filePath, json);
                var config = new ErrorReportConfiguration(directory, processedFolder);
                var container = GetErrorReportReporter(config);
                var errorReportReporter = container.ErrorReportReporter;

                //When
                errorReportReporter.Start();

                //Then
                File.Exists(expectedFilePath).Should().BeTrue();
            });
        }

        [Test, Category(Categories.Unit)]
        public void ErrorReportReporterStartsOnceWhenStartingItTwice()
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

        [Test, Category(Categories.Unit)]
        public void ErrorReportIsProcessedWhenItIsCreatedAsFileOnDrive()
        {
            StorageManager.InTemporaryDirectory(directory => 
            {
                //Given
                var config = new ErrorReportConfiguration(directory, Path.Combine(directory, "processed"));
                var container = GetErrorReportReporter(config);
                var errorReportReporter = container.ErrorReportReporter;
                var gitHubIssueCreatorMock = container.GitHubIssueCreatorMock;
                gitHubIssueCreatorMock.Setup(gic => gic.CreateIssue(It.IsAny<ErrorReport>()));
                var fileSystemWatcherMock = container.FileSystemWatcherMock;
                var eventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, directory, "error-report.json");
                
                var errorReport = new ErrorReport();
                var json = JsonConvert.SerializeObject(errorReport);
                var filePath = Path.Combine(directory, "error-report.json");
                errorReportReporter.Start();

                //When
                File.WriteAllText(filePath, json);
                fileSystemWatcherMock.Raise(fsw => fsw.Created += null, eventArgs);

                //Then
                gitHubIssueCreatorMock.Verify(gic => gic.CreateIssue(It.IsAny<ErrorReport>()), Times.Once);
            });
        }

        //Pretty far fetched test, but was quick to write as we had the test above already.
        //Writing this comment took the most time :P
        [Test, Category(Categories.Unit)]
        public void ErrorReportIsNotProcessedWhenItDoesNotExistOnDiskThoughFileSystemWatcherRaisedCreatedEvent()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                var config = new ErrorReportConfiguration(directory, Path.Combine(directory, "processed"));
                var container = GetErrorReportReporter(config);
                var errorReportReporter = container.ErrorReportReporter;
                var gitHubIssueCreatorMock = container.GitHubIssueCreatorMock;
                gitHubIssueCreatorMock.Setup(gic => gic.CreateIssue(It.IsAny<ErrorReport>()));
                var fileSystemWatcherMock = container.FileSystemWatcherMock;
                var eventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, directory, "error-report.json");
                errorReportReporter.Start();

                //When
                fileSystemWatcherMock.Raise(fsw => fsw.Created += null, eventArgs);

                //Then
                gitHubIssueCreatorMock.Verify(gic => gic.CreateIssue(It.IsAny<ErrorReport>()), Times.Never);
            });
        }

        [Test, Category(Categories.Unit)]
        public void InvalidErrorReportCausesNoException()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                var config = new ErrorReportConfiguration(directory, Path.Combine(directory, "processed"));
                var container = GetErrorReportReporter(config);
                var errorReportReporter = container.ErrorReportReporter;
                var gitHubIssueCreatorMock = container.GitHubIssueCreatorMock;
                gitHubIssueCreatorMock.Setup(gic => gic.CreateIssue(It.IsAny<ErrorReport>()));
                var fileSystemWatcherMock = container.FileSystemWatcherMock;
                var eventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, directory, "error-report.json");

                var filePath = Path.Combine(directory, "error-report.json");
                errorReportReporter.Start();

                //When
                File.WriteAllText(filePath, "eijef9*()f397");
                fileSystemWatcherMock.Raise(fsw => fsw.Created += null, eventArgs);

                //Then
                gitHubIssueCreatorMock.Verify(gic => gic.CreateIssue(It.IsAny<ErrorReport>()), Times.Never);
            });
        }

        #region Helpers
        private static GetContainer GetErrorReportReporter(ErrorReportConfiguration errorReportConfiguration)
        {
            var fileSystemWatcherMock = new Mock<IFileSystemWatcher>();
            var gitHubIssueCreatorMock = new Mock<IGitHubIssueCreator>();
            var errorReportReporter = new ErrorReportReporter(fileSystemWatcherMock.Object, errorReportConfiguration, gitHubIssueCreatorMock.Object);
            return new GetContainer(fileSystemWatcherMock, gitHubIssueCreatorMock, errorReportReporter, errorReportConfiguration);
        }

        private class GetContainer
        {
            public Mock<IFileSystemWatcher> FileSystemWatcherMock { get; private set; }
            public ErrorReportReporter ErrorReportReporter { get; private set; }
            public ErrorReportConfiguration ErrorReportConfiguration { get; private set; }
            public Mock<IGitHubIssueCreator> GitHubIssueCreatorMock { get; private set; }

            public GetContainer(Mock<IFileSystemWatcher> fileSystemWatcherMock, Mock<IGitHubIssueCreator> gitHubIssueCreatorMock, ErrorReportReporter errorReportReporter, ErrorReportConfiguration errorReportConfiguration)
            {
                FileSystemWatcherMock = fileSystemWatcherMock;
                GitHubIssueCreatorMock = gitHubIssueCreatorMock;
                ErrorReportReporter = errorReportReporter;
                ErrorReportConfiguration = errorReportConfiguration;
            }
        }
        #endregion
    }
}
