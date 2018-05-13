using System;
using System.IO;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Newtonsoft.Json;

namespace CreateMask.Workers
{
    public class ErrorReportCreator : IErrorReportCreator
    {
        private readonly ErrorReport _errorReport = new ErrorReport();
        private string _fileName;

        public void CreateReport(Version version, Exception exception, ApplicationArguments applicationArguments, string fileName)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (applicationArguments == null) throw new ArgumentNullException(nameof(applicationArguments));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));

            _fileName = fileName;
            _errorReport.Version = version;
            _errorReport.Exception = exception;
            _errorReport.ApplicationArguments = applicationArguments;

            LoadFiles();

            SerializeToFile();
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
            using (var file = File.CreateText(_fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, _errorReport);
            }
        }
    }
}
