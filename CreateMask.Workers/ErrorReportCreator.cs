using System;
using System.IO;
using System.Linq.Expressions;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Newtonsoft.Json;

namespace CreateMask.Workers
{
    public class ErrorReportCreator : IErrorReportCreator
    {
        private readonly ErrorReport _errorReport = new ErrorReport();
        private string _errorReportsDirectory;

        public void CreateReport(Version version, Exception exception, ApplicationArguments applicationArguments, string errorReportsDirectory)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (applicationArguments == null) throw new ArgumentNullException(nameof(applicationArguments));
            if (errorReportsDirectory == null) throw new ArgumentNullException(nameof(errorReportsDirectory));

            _errorReportsDirectory = errorReportsDirectory;
            _errorReport.Version = version;
            _errorReport.Exception = exception;
            _errorReport.ApplicationArguments = applicationArguments;

            EnsureDirectoryExists();

            LoadFiles();

            SerializeToFile();
        }

        private void EnsureDirectoryExists()
        {
            if (Directory.Exists(_errorReportsDirectory)) return;
            Directory.CreateDirectory(_errorReportsDirectory);
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
            var dateTimeMilliseconds = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
            var fileName = dateTimeMilliseconds + ".json";
            var filePath = Path.Combine(_errorReportsDirectory, fileName);
            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, _errorReport);
            }
        }
    }
}
