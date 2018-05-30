using System;
using System.IO;
using System.Threading;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Newtonsoft.Json;

namespace CreateMask.Workers
{
    public class ErrorReportReporter : IErrorReportReporter
    {
        private readonly IFileSystemWatcher _fileSystemWatcher;
        private readonly ErrorReportConfiguration _errorReportConfiguration;
        private readonly IGitHubIssueCreator _gitHubIssueCreator;
        private bool _started;

        public ErrorReportReporter(IFileSystemWatcher fileSystemWatcher, 
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

            EnsureReportedDirectoryExists();

            _fileSystemWatcher.Created += OnErrorReportCreated;
            _fileSystemWatcher.Start(_errorReportConfiguration.MainDirectory);

            ProcessAlreadyPresentErrorReports();
        }

        public void EnsureReportedDirectoryExists()
        {
            if (!Directory.Exists(_errorReportConfiguration.ReportedDirectory))
            {
                Directory.CreateDirectory(_errorReportConfiguration.ReportedDirectory);
            }
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
                ProcessErrorReport(file);                
            }
        }

        private void ProcessErrorReport(string filePath)
        {
            try
            {
                var errorReport = DeserializeErrorReport(filePath);
                if (errorReport == null)
                {
                    File.Delete(filePath); //Empty error report file or incorrupt.
                    return;
                }
                _gitHubIssueCreator.CreateIssue(errorReport);

                MoveErrorReport(filePath);
            }
            catch
            {
                //Too bad, what can we do.
                //Don't want to be stuck in endless loop or trying to report exceptions of failing reporting.
            }
        }

        private static ErrorReport DeserializeErrorReport(string filePath)
        {
            const int retries = 5;
            Exception lastException = null;
            for (var i = 0; i < retries; i++)
            {
                try
                {
                    var fileContents = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<ErrorReport>(fileContents);
                }
                catch (IOException ex)
                {
                    lastException = ex;
                    Thread.Sleep(200);
                }
            }
            throw lastException;
        }

        private void MoveErrorReport(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var newFilePath = Path.Combine(_errorReportConfiguration.ReportedDirectory, fileName);
            File.Move(filePath, newFilePath);
        }
    }
}
