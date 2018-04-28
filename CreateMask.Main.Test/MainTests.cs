using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
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
            var applicationArguments = new ApplicationArguments();
            applicationArguments.DesiredResistance = 8820;
            applicationArguments.High = 255;
            applicationArguments.LcdHeight = 1440;
            applicationArguments.LcdMeasurementsFilePathHigh = FileManager.GetFullFilePath("high.csv");
            applicationArguments.LcdMeasurementsFilePathLow = FileManager.GetFullFilePath("low.csv");
            applicationArguments.LcdWidth = 2560;
            applicationArguments.LdrCalibrationFilePath = FileManager.GetFullFilePath("ldrcurve.csv");
            applicationArguments.Low = 175;
            applicationArguments.MaskFilePath = Path.GetFullPath("./mask2.png");
            applicationArguments.MeasurementsNrOfColumns = 12;
            applicationArguments.MeasurementsNrOfRows = 7;
            applicationArguments.FileType = ".png";
            applicationArguments.OriginalExposureTime = 8000;
            return applicationArguments;
        }
    }
}
