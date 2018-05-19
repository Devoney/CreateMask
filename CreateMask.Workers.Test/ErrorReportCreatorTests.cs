using CreateMask.Containers;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Workers.Test
{
    [TestFixture]
    public class ErrorReportCreatorTests
    {
        [Test, Category(Categories.Unit)]
        public void ErrorReportIsSerializedToFileOnStorage()
        {
            //Given

            //When

            //Then
            Assert.Fail("todo");
        }

        [Test, Category(Categories.Unit)]
        public void SerializedErrorReportContainsCorrectData()
        {
            //Given

            //When

            //Then
            Assert.Fail("todo");
        }

        [Test, Category(Categories.Unit)]
        [TestCase(nameof(ApplicationArguments.LdrCalibrationFilePath), nameof(ErrorReport.LdrCurveCsvData))]
        [TestCase(nameof(ApplicationArguments.LcdMeasurementsFilePathLow), nameof(ErrorReport.MeasurementsLowCsvData))]
        [TestCase(nameof(ApplicationArguments.LcdMeasurementsFilePathHigh), nameof(ErrorReport.MeasurementsHighCsvData))]
        public void FileContentsIsAddedToErrorReport(string applicationArgumentsPropertyName, string errorReportPropertyName)
        {
            //Given

            //When

            //Then
            Assert.Fail("todo");
        }

        [Test, Category(Categories.Unit)]
        [TestCase(nameof(ApplicationArguments.LdrCalibrationFilePath), nameof(ErrorReport.LdrCurveCsvData))]
        [TestCase(nameof(ApplicationArguments.LcdMeasurementsFilePathLow), nameof(ErrorReport.MeasurementsLowCsvData))]
        [TestCase(nameof(ApplicationArguments.LcdMeasurementsFilePathHigh), nameof(ErrorReport.MeasurementsHighCsvData))]
        public void WhenFileDoesNotExistItsContentsIsNull(string applicationArgumentsPropertyName, string errorReportPropertyName)
        {
            //Given

            //When

            //Then
            Assert.Fail("todo");
        }
    }
}
