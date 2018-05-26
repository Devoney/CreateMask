using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using Moq;
using Ninject;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Main.Test
{
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
            using (var expectedBitmap = new Bitmap(Image.FromFile(StorageManager.GetFullFilePath("MaskIsCreatedAsExpected.png"))))
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

        [Test, Category(Categories.Unit)]
        public void ErrorReportIsCreatedUponException()
        {
            //Given
            var items = GetFullyMockedMain();
            var mocks = items.Item1;

            var errorReportCreatorMock = mocks.ErrorReportCreator;
            errorReportCreatorMock.Setup(
                ercm =>
                    ercm.CreateReport(It.IsAny<Version>(),
                    It.IsAny<Exception>(),
                    It.IsAny<ApplicationArguments>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()));

            mocks.OutputWriter.Setup(ow => ow.LoadingFile(It.IsAny<string>()))
                .Throws<FileNotFoundException>();

            var main = items.Item2;
            var applictionArguments = GetApplicationArguments(); //This will cause the exception

            //When
            try
            {
                main.CreateMask(applictionArguments);
            }
            catch
            {
                // ignored
            }

            //Then
            errorReportCreatorMock.Verify(ercm => ercm.CreateReport(
                It.IsAny<Version>(), 
                It.IsAny<Exception>(), 
                It.IsAny<ApplicationArguments>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Test, Category(Categories.Unit)]
        public void ExceptionThrownInErrorReportIsHidden()
        {
            //Given
            const string exceptionMessage = "Exception to test";
            var exceptionToThrow = new Exception(exceptionMessage);
            var items = GetFullyMockedMain();
            var mocks = items.Item1;
            var main = items.Item2;

            var errorReportCreatorMock = mocks.ErrorReportCreator;
            errorReportCreatorMock.Setup(
                ercm =>
                    ercm.CreateReport(
                        It.IsAny<Version>(),
                        It.IsAny<Exception>(), 
                        It.IsAny<ApplicationArguments>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                        .Throws<Exception>();

            mocks.BitmapProcessor
                .Setup(bp => bp.Save(It.IsAny<Bitmap>(), It.IsAny<string>(), It.IsAny<ImageFormat>()))
                .Throws(exceptionToThrow);

            var applicationArguments = GetApplicationArguments();

            //When
            var action = new Action(() =>
            {
                main.CreateMask(applicationArguments);
            });


            //Then
            AssertExt.ThrowsException<Exception>(action, exceptionMessage);

            errorReportCreatorMock.Verify(erc => erc.CreateReport(
                It.IsAny<Version>(), 
                It.IsAny<Exception>(), 
                It.IsAny<ApplicationArguments>(), 
                It.IsAny<string>(),
                It.IsAny<string>()), 
                Times.Once);
        }

        #region Helpers

        private class MockedObjects
        {
            public Mock<IBitmapProcessor> BitmapProcessor { get; set; }
            public Mock<IErrorReportCreator> ErrorReportCreator { get; set; }
            public Mock<IExposureTimeCalculator> ExposureTimeCalculator { get; set; }
            public Mock<IGenericGridLoader<int>> GenericGridLoader { get; set; }
            public Mock<IMaskIntensityResistanceInterpolatorFactory> MaskIntensityResistanceInterpolatorFactory { get; set; }
            public Mock<IMeasurementGridProcessor> MeasurementGridProcessor { get; set; }
            public Mock<IGenericLoader<Measurement>> MeasurementsLoader { get; set; }
            public Mock<IOutputWriter> OutputWriter { get; set; }
            public Mock<IErrorReportReporter> ErrorReportReporter { get; set; }
        }
        private Tuple<MockedObjects, Main> GetFullyMockedMain()
        {
            var measuremntsLoader = new Mock<IGenericLoader<Measurement>>();
            var factory = new Mock<IMaskIntensityResistanceInterpolatorFactory>();
            var gridLoader = new Mock<IGenericGridLoader<int>>();
            var gridProcessor = new Mock<IMeasurementGridProcessor>();
            var exposureTimeCalculator = new Mock<IExposureTimeCalculator>();
            var outputWriter = new Mock<IOutputWriter>();
            var errorReportCreator = new Mock<IErrorReportCreator>();
            var bitmapProcessor = new Mock<IBitmapProcessor>();
            var errorReportReporter = new Mock<IErrorReportReporter>();

            var mockedObjects = new MockedObjects
            {
                BitmapProcessor = bitmapProcessor,
                ErrorReportCreator = errorReportCreator,
                ExposureTimeCalculator = exposureTimeCalculator,
                GenericGridLoader = gridLoader,
                MaskIntensityResistanceInterpolatorFactory = factory,
                MeasurementGridProcessor = gridProcessor,
                MeasurementsLoader = measuremntsLoader,
                OutputWriter = outputWriter,
                ErrorReportReporter = errorReportReporter
            };

            var main = new Main(
                measuremntsLoader.Object, 
                factory.Object, 
                gridLoader.Object, 
                gridProcessor.Object, 
                exposureTimeCalculator.Object, 
                outputWriter.Object,
                bitmapProcessor.Object,
                errorReportCreator.Object,
                new ErrorReportConfiguration("./error-reports", "./error-reports/reported"),
                errorReportReporter.Object);

            return new Tuple<MockedObjects, Main>(mockedObjects, main);
        }

        private string GetFullPath(string filePath, string format = OutputStrings.LoadingFile)
        {
            return string.Format(format, Path.GetFullPath(filePath));
        }

        private static Main GetMain(IErrorReportCreator errorReportCreator = null)
        {
            var kernel = KernelConstructor.GetKernel();
            if (errorReportCreator != null)
            {
                kernel.Unbind<IErrorReportCreator>();
                kernel.Bind<IErrorReportCreator>().ToConstant(errorReportCreator);
            }
            return kernel.Get<Main>();
        }

        private static ApplicationArguments GetApplicationArguments()
        {
            var applicationArguments = new ApplicationArguments
            {
                DesiredResistance = 8820,
                High = 255,
                LcdHeight = 1440,
                LcdMeasurementsFilePathHigh = StorageManager.GetFullFilePath("high.csv"),
                LcdMeasurementsFilePathLow = StorageManager.GetFullFilePath("low.csv"),
                LcdWidth = 2560,
                LdrCalibrationFilePath = StorageManager.GetFullFilePath("ldrcurve.csv"),
                Low = 175,
                MaskFilePath = Path.GetFullPath("./mask2.png"),
                MeasurementsNrOfColumns = 12,
                MeasurementsNrOfRows = 7,
                FileType = ".png",
                OriginalExposureTime = 8000
            };
            return applicationArguments;
        }
        #endregion
    }
}
