using System.Drawing;
using System.IO;
using System.Text;
using CreateMask.Containers;
using FluentAssertions;
using Ninject;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Main.Test
{
    /// <summary>
    /// Summary description for MainTests
    /// </summary>
    [TestFixture]
    public class MainTests
    {
        [Test, Category(Categories.Integration)]
        public void MaskIsCreatedAsExpected()
        {
            //Given
            var applicationArguments = GetApplicationArguments();
            applicationArguments.MaskFilePath = "./mask.png";
            var main = GetMain();

            //When
            main.CreateMask(applicationArguments);

            //Then
            using (var actualBitmap = new Bitmap(Image.FromFile(applicationArguments.MaskFilePath)))
            using (var expectedBitmap = new Bitmap(Image.FromFile(FileManager.GetFullFilePath("MaskIsCreatedAsExpected.png"))))
            {
                AssertExt.Equals(expectedBitmap, actualBitmap);
            }
        }

        [Test, Category(Categories.Integration)]
        public void OutputIsAsExpected()
        {
            //Given
            var applicationArguments = GetApplicationArguments();
            var main = GetMain();
            var outputStringBuilder = new StringBuilder();
            main.Output += (sender, output) =>
            {
                outputStringBuilder.AppendLine(output);
            };
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(GetFullPath(applicationArguments.LdrCalibrationFilePath));
            stringBuilder.AppendLine(OutputStrings.ConstructionLdPolynomialCurveFit);
            stringBuilder.AppendLine(GetFullPath(applicationArguments.LcdMeasurementsFilePathHigh));
            stringBuilder.AppendLine(GetFullPath(applicationArguments.LcdMeasurementsFilePathLow));
            stringBuilder.AppendLine(OutputStrings.ConstructingGridOfLowHighMeasurements);
            stringBuilder.AppendLine(OutputStrings.CreatingGridOfLocalMaskIntensities);
            stringBuilder.AppendLine(OutputStrings.ConvertingLocalMaskIntensitiesToBitmap);
            stringBuilder.AppendLine(string.Format(OutputStrings.ResizingBitmap,
                applicationArguments.MeasurementsNrOfColumns,
                applicationArguments.MeasurementsNrOfRows,
                applicationArguments.LcdWidth,
                applicationArguments.LcdHeight));
            stringBuilder.AppendLine(GetFullPath(applicationArguments.MaskFilePath, OutputStrings.MaskSavedTo));
            stringBuilder.AppendLine(string.Format(OutputStrings.NewAdvisedExposureTime, "10025"));

            //When
            main.CreateMask(applicationArguments);

            //Then
            var actualOutput = outputStringBuilder.ToString();
            var expectedOutput = stringBuilder.ToString();
            actualOutput.Should().Be(expectedOutput);
        }

        [Test, Category(Categories.Unit)]
        public void ExposureTimeIsNotCalculatedWhenOriginalExposureTimeIsNotSet()
        {
            //Given
            var applicationArguments = GetApplicationArguments();
            applicationArguments.MaskFilePath = "./mask3.png";
            applicationArguments.OriginalExposureTime = 0;
            var main = GetMain();
            var outputStringBuilder = new StringBuilder();
            main.Output += (sender, s) =>
            {
                outputStringBuilder.AppendLine(s);
            };
            var notExpectedString = OutputStrings.NewAdvisedExposureTime.Replace("{0}", "");

            //When
            main.CreateMask(applicationArguments);
            
            //Then
            var output = outputStringBuilder.ToString();
            var containsAdvisedExposureTime = output.Contains(notExpectedString);
            containsAdvisedExposureTime.Should().BeFalse();
        }

        [Test, Category(Categories.Unit)]
        public void CorrectSupportedFileTypesAreReturned()
        {
            //Given
            var expectedSupportedFileTypes = new[]
            {
                "Bmp",
                "Gif",
                "Jpeg",
                "Png",
                "Tiff"
            };
            var main = GetMain();

            //When
            var actualSupportedFileTypes = main.SupportedFileTypes;

            //Then
            actualSupportedFileTypes.Should().BeEquivalentTo(expectedSupportedFileTypes);
        }

        private string GetFullPath(string filePath, string format = OutputStrings.LoadingFile)
        {
            return string.Format(format, Path.GetFullPath(filePath));
        }

        private static Main GetMain()
        {
            return KernelConstructor.GetKernel().Get<Main>();
        }

        private static ApplicationArguments GetApplicationArguments()
        {
            var applicationArguments = new ApplicationArguments
            {
                DesiredResistance = 8820,
                High = 255,
                LcdHeight = 1440,
                LcdMeasurementsFilePathHigh = FileManager.GetFullFilePath("high.csv"),
                LcdMeasurementsFilePathLow = FileManager.GetFullFilePath("low.csv"),
                LcdWidth = 2560,
                LdrCalibrationFilePath = FileManager.GetFullFilePath("ldrcurve.csv"),
                Low = 175,
                MaskFilePath = Path.GetFullPath("./mask2.png"),
                MeasurementsNrOfColumns = 12,
                MeasurementsNrOfRows = 7,
                FileType = ".png",
                OriginalExposureTime = 8000
            };
            return applicationArguments;
        }
    }
}
