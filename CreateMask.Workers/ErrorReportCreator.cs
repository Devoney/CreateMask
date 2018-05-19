using System;
using System.IO;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Newtonsoft.Json;

namespace CreateMask.Workers
{
    public class ErrorReportCreator : IErrorReportCreator
    {
        private readonly IDateTimeWorker _dateTimeWorker;
        private readonly ErrorReport _errorReport = new ErrorReport();
        private string _directory;
        private string _reportName;

        public ErrorReportCreator(IDateTimeWorker dateTimeWorker)
        {
            if (dateTimeWorker == null) throw new ArgumentNullException(nameof(dateTimeWorker));
            _dateTimeWorker = dateTimeWorker;
        }

        public void CreateReport(
            Version version, 
            Exception exception, 
            ApplicationArguments applicationArguments, 
            string directory, 
            string reportName)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (applicationArguments == null) throw new ArgumentNullException(nameof(applicationArguments));
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (reportName == null) throw new ArgumentNullException(nameof(reportName));

            _directory = directory;
            _reportName = reportName;
            
            _errorReport.DateTime = _dateTimeWorker.Now;
            _errorReport.Version = version;
            _errorReport.Exception = exception;
            _errorReport.ApplicationArguments = applicationArguments;

            LoadFiles();

            RemoveSensitivePersonalInformation();

            EnsureDirectoryExists();

            SerializeToFile();
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);
        }

        private void RemoveSensitivePersonalInformation()
        {
            if (!string.IsNullOrEmpty(_errorReport.ApplicationArguments.MaskFilePath))
            {
                _errorReport.ApplicationArguments.MaskFilePath = "[REMOVED]";
            }
            if (!string.IsNullOrEmpty(_errorReport.ApplicationArguments.LdrCalibrationFilePath))
            {
                _errorReport.ApplicationArguments.LdrCalibrationFilePath = "[REMOVED]";
            }
            if (!string.IsNullOrEmpty(_errorReport.ApplicationArguments.LcdMeasurementsFilePathHigh))
            {
                _errorReport.ApplicationArguments.LcdMeasurementsFilePathHigh = "[REMOVED]";
            }
            if (!string.IsNullOrEmpty(_errorReport.ApplicationArguments.LcdMeasurementsFilePathLow))
            {
                _errorReport.ApplicationArguments.LcdMeasurementsFilePathLow = "[REMOVED]";
            }
        }

        private void LoadFiles()
        {
            _errorReport.LdrCurveCsvData = LoadFile(_errorReport.ApplicationArguments.LdrCalibrationFilePath);
            _errorReport.MeasurementsLowCsvData = LoadFile(_errorReport.ApplicationArguments.LcdMeasurementsFilePathLow);
            _errorReport.MeasurementsHighCsvData = LoadFile(_errorReport.ApplicationArguments.LcdMeasurementsFilePathHigh);
        }

        private string LoadFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            return File.ReadAllText(filePath);
        }

        private void SerializeToFile()
        {
            var fileName = Path.Combine(_directory, _reportName) + ".json";

            using (var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, _errorReport);
            }
        }
    }
}
