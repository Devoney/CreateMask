using System.IO;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Newtonsoft.Json;

namespace CreateMask.Workers
{
    public class ErrorReportProcessor : IErrorReportProcessor
    {
        private readonly IFileSystemWatcher _fileSystemWatcher;
        private readonly ErrorReportConfiguration _errorReportConfiguration;
        private readonly IGitHubIssueCreator _gitHubIssueCreator;
        private bool _started;

        public ErrorReportProcessor(IFileSystemWatcher fileSystemWatcher, 
            ErrorReportConfiguration errorReportConfiguration,
            IGitHubIssueCreator gitHubIssueCreator)
        {
            _fileSystemWatcher = fileSystemWatcher;
            _errorReportConfiguration = errorReportConfiguration;
            _gitHubIssueCreator = gitHubIssueCreator;
        }

        public void Start()
        {
            if (_started) return;
            _started = true;

            _fileSystemWatcher.Created += OnErrorReportCreated;
            _fileSystemWatcher.Start(_errorReportConfiguration.MainDirectory);

            ProcessAlreadyPresentErrorReports();
        }

        private void OnErrorReportCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            var filePath = fileSystemEventArgs.FullPath;
            if (!File.Exists(filePath)) return; //Would be strange but you never know.

            ProcessErrorReport(filePath);
        }

        private void ProcessAlreadyPresentErrorReports()
        {
            var files = Directory.GetFiles(_errorReportConfiguration.MainDirectory, "*.json");
            foreach (var file in files)
            {
                try
                {
                    ProcessErrorReport(file);
                }
                catch
                {
                    //Just continue with the next
                }
            }
        }

        private void ProcessErrorReport(string filePath)
        {
            var errorReport = DeserializeErrorReport(filePath);
            _gitHubIssueCreator.CreateIssue(errorReport);

            MoveErrorReport(filePath);
        }

        private static ErrorReport DeserializeErrorReport(string filePath)
        {
            var fileContents = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<ErrorReport>(fileContents);
        }

        private void MoveErrorReport(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var newFilePath = Path.Combine(directory, _errorReportConfiguration.ReportedDirectory, fileName);
            File.Move(filePath, newFilePath);
        }
    }
}
