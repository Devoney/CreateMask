using System;
using System.IO;
using System.Linq;
using System.Security.Policy;
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
    public class ErrorReportCreatorTests
    {
        [Test, Category(Categories.Unit)]
        public void DirectoryIsCreatedWhenItDoesNotExistYet()
        {
            //Given
            var errorReportCreator = GetErrorReportCreator();
            var version = new Version();
            var exception = new Exception();
            var applicationArguments = new ApplicationArguments();
            const string directoryName = "./IEJDKF8eF";
            if(Directory.Exists(directoryName)) Directory.Delete(directoryName, true);
            const string reportName = "39837DJ";

            //When
            errorReportCreator.CreateReport(version, exception, applicationArguments, directoryName, reportName);

            //Then
            Directory.Exists(directoryName).Should().BeTrue();
        }

        [Test, Category(Categories.Unit)]
        public void ErrorReportIsSerializedToFileOnStorageWithoutCsvFilesSpecifiedInApplicationArguments()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                const string reportName = "DKEFN-09-87DF";
                var errorReportCreator = GetErrorReportCreator();
                var exception = new Exception();
                var version = new Version();
                var applicationArguments = new ApplicationArguments();

                //When
                errorReportCreator.CreateReport(version, exception, applicationArguments, directory, reportName);

                //Then
                var filesInDirectory = Directory.GetFiles(directory, reportName + "*").ToList();
                filesInDirectory.Count.Should().Be(1);
            });
        }

        [Test, Category(Categories.Unit)]
        public void SerializedErrorReportContainsCorrectData()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                using (var tempFileManager = new TempFileManager(directory))
                {
                    //Given
                    const string reportName = "JEU76-12-95FE";
                    const string expectedFileName = "JEU76-12-95FE_expected.json";
                    var errorReportCreator = GetErrorReportCreator();
                    const string exceptionMessage = reportName + " exception message";
                    var exception = new Exception(exceptionMessage);
                    var version = new Version(1, 2, 3, 4);
                    var applicationArguments = new ApplicationArguments();

                    const string ldrContents = "Ldr file contents";
                    const string highContents = "High file contents";
                    const string lowContents = "Low file contents";
                    applicationArguments.DesiredResistance = 8820;
                    applicationArguments.FileType = "bmp";
                    applicationArguments.High = 20;
                    applicationArguments.Low = 128;
                    applicationArguments.LcdHeight = 1440;
                    applicationArguments.LcdWidth = 2560;
                    applicationArguments.MaskFilePath = @"c:\user\my\dir\mask.bmp";
                    applicationArguments.OriginalExposureTime = 1540;
                    applicationArguments.MeasurementsNrOfColumns = 7;
                    applicationArguments.MeasurementsNrOfRows = 9;
                    applicationArguments.LdrCalibrationFilePath = tempFileManager.GetTempFile(ldrContents);
                    applicationArguments.LcdMeasurementsFilePathHigh = tempFileManager.GetTempFile(highContents);
                    applicationArguments.LcdMeasurementsFilePathLow = tempFileManager.GetTempFile(lowContents);

                    //When
                    errorReportCreator.CreateReport(version, exception, applicationArguments, directory, reportName);

                    //Then
                    var filesInDirectory = Directory.GetFiles(directory, reportName + "*").ToList();
                    filesInDirectory.Count.Should().Be(1);
                    var report = filesInDirectory.Single();

                    var actualFileContents = File.ReadAllText(report);
                    var expectedFilePath = StorageManager.GetFullFilePath(expectedFileName);
                    var expectedFileContents = File.ReadAllText(expectedFilePath);
                    actualFileContents.Should().Be(expectedFileContents);
                }
            });
        }

        [Test, Category(Categories.Unit)]
        [TestCase(nameof(ApplicationArguments.LdrCalibrationFilePath), nameof(ErrorReport.LdrCurveCsvData))]
        [TestCase(nameof(ApplicationArguments.LcdMeasurementsFilePathLow), nameof(ErrorReport.MeasurementsLowCsvData))]
        [TestCase(nameof(ApplicationArguments.LcdMeasurementsFilePathHigh), nameof(ErrorReport.MeasurementsHighCsvData))]
        public void WhenFileDoesNotExistItsContentsIsNull(string applicationArgumentsPropertyName, string errorReportPropertyName)
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                using (var tempFileManager = new TempFileManager(directory))
                {
                    //Given
                    const string reportName = "JEU76-12-95FE";
                    var errorReportCreator = GetErrorReportCreator();
                    const string exceptionMessage = reportName + " exception message";
                    var exception = new Exception(exceptionMessage);
                    var version = new Version(1, 2, 3, 4);
                    var applicationArguments = new ApplicationArguments();

                    const string ldrContents = "Ldr file contents";
                    const string highContents = "High file contents";
                    const string lowContents = "Low file contents";
                    applicationArguments.DesiredResistance = 8820;
                    applicationArguments.FileType = "bmp";
                    applicationArguments.High = 20;
                    applicationArguments.Low = 128;
                    applicationArguments.LcdHeight = 1440;
                    applicationArguments.LcdWidth = 2560;
                    applicationArguments.MaskFilePath = @"c:\user\my\dir\mask.bmp";
                    applicationArguments.OriginalExposureTime = 1540;
                    applicationArguments.MeasurementsNrOfColumns = 7;
                    applicationArguments.MeasurementsNrOfRows = 9;
                    if (applicationArgumentsPropertyName != nameof(ApplicationArguments.LdrCalibrationFilePath))
                    {
                        applicationArguments.LdrCalibrationFilePath = tempFileManager.GetTempFile(ldrContents);
                    }
                    if (applicationArgumentsPropertyName != nameof(ApplicationArguments.LcdMeasurementsFilePathHigh))
                    {
                        applicationArguments.LcdMeasurementsFilePathHigh = tempFileManager.GetTempFile(highContents);
                    }
                    if (applicationArgumentsPropertyName != nameof(ApplicationArguments.LcdMeasurementsFilePathLow))
                    {
                        applicationArguments.LcdMeasurementsFilePathLow = tempFileManager.GetTempFile(lowContents);
                    }

                    //When
                    errorReportCreator.CreateReport(version, exception, applicationArguments, directory, reportName);

                    //Then
                    var filesInDirectory = Directory.GetFiles(directory, reportName + "*").ToList();
                    filesInDirectory.Count.Should().Be(1);
                    var report = filesInDirectory.Single();

                    var actualFileContents = File.ReadAllText(report);
                    var actualErrorReport = JsonConvert.DeserializeObject<ErrorReport>(actualFileContents);

                    var propertyInfo = typeof (ErrorReport).GetProperty(errorReportPropertyName);
                    var propertyValue = propertyInfo.GetValue(actualErrorReport);
                    propertyValue.Should().BeNull();
                }
            });
        }

        private IErrorReportCreator GetErrorReportCreator()
        {
            var dateTimeWorkerMock = new Mock<IDateTimeWorker>();
            dateTimeWorkerMock.Setup(dtw => dtw.Now).Returns(new DateTime(2018, 05, 19));
            var dateTimeWorker = dateTimeWorkerMock.Object;
            return new ErrorReportCreator(dateTimeWorker);
        }
    }
}
