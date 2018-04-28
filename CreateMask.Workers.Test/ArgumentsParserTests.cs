using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using CreateMask.Containers;
using FluentAssertions;
using NUnit.Framework;
using TestHelpers;
using Args = CreateMask.Contracts.Constants.Arguments;

namespace CreateMask.Workers.Test
{
    [TestFixture]
    public class ArgumentsParserTests
    {
        private const int LcdWidth = 2560;
        private const int LcdHeight = 1440;
        private const string LdrCalibrationFilePath = @"C:\LdrCalibration.csv";
        private const string LcdMeasurementsFilePathHigh = @"C:\LcdMeasurements_high.csv";
        private const string LcdMeasurementsFilePathLow = @"C:\LcdMeasurements_low.csv";
        private const string MaskFilePath = @"C:\Mask.png";
        private const int NrOfRows = 7;
        private const int NrOfColumns = 12;
        private const int High = 255;
        private const int Low = 127;
        private const int DesiredResistance = 5467;
        private const string FileType = ".bmp";
        private const int OriginalExposureTime = 8000;

        private readonly List<string> _supportedFileTypes = new List<string>
        {
            nameof(ImageFormat.Bmp),
            nameof(ImageFormat.Png)
        }; 

        private readonly string _commandLine = $"--{Args.LcdWidth} {LcdWidth} " +
                                               $"--{Args.LcdHeight} {LcdHeight} " +
                                               $"--{Args.LdrCalibrationFilePath} \"{LdrCalibrationFilePath}\" " +
                                               $"--{Args.LcdMeasurementsFilePathHigh} \"{LcdMeasurementsFilePathHigh}\" " +
                                               $"--{Args.LcdMeasurementsFilePathLow} \"{LcdMeasurementsFilePathLow}\" " +
                                               $"--{Args.High} {High} " +
                                               $"--{Args.Low} {Low} " +
                                               $"--{Args.MaskFilePath} \"{MaskFilePath}\" " +
                                               $"--{Args.MeasurementsNrOfRows} {NrOfRows} " +
                                               $"--{Args.MeasurementsNrOfColumns} {NrOfColumns} " +
                                               $"--{Args.DesiredResistance} {DesiredResistance} " +
                                               $"--{Args.FileType} {FileType} " +
                                               $"--{Args.OriginalExposureTime} {OriginalExposureTime}";

        private readonly string[] _args;

        public ArgumentsParserTests()
        {
            _args = _commandLine.Split(' ');
        }

        [Test, Category(Categories.Unit)]
        public void ArgumentsAreParsedCorrectly()
        {
            //Given
            var expectedArguments = new ApplicationArguments
            {
                LcdWidth = LcdWidth,
                LcdHeight = LcdHeight,
                LdrCalibrationFilePath = LdrCalibrationFilePath,
                LcdMeasurementsFilePathHigh = LcdMeasurementsFilePathHigh,
                LcdMeasurementsFilePathLow = LcdMeasurementsFilePathLow,
                High = High,
                Low = Low,
                MaskFilePath = MaskFilePath,
                MeasurementsNrOfRows = NrOfRows,
                MeasurementsNrOfColumns = NrOfColumns,
                DesiredResistance = DesiredResistance,
                FileType = FileType,
                OriginalExposureTime = OriginalExposureTime
            };
            var argumentsParser = GetArgumentsParser();

            //When
            var actualArguments = argumentsParser.Parse(_args, _supportedFileTypes);

            //Then
            actualArguments.Should().BeEquivalentTo(expectedArguments);
        }

        [Test, Category(Categories.Unit)]
        [TestCase("?")]
        [TestCase("help")]
        [TestCase("-help")]
        [TestCase("--help")]
        public void HelpForArgumentsIsShownWhenNoOrHelpArgumentsAreSet(string helpArgument)
        {
            //Given
            var expectedHelpTexts = new []
            {
                $"--{Args.DesiredResistance}\t\tThe resistance value you want to normalize the entire LCD screen to. This determines the increase in exposure time.",
                $"--{Args.FileType}\t\tThe type of file to output. Supported file types are: Bmp, Png",
                $"--{Args.High}\t\t[OPTIONAL, DEFAULT=255] The pixel value(0-255) of the mask used for the 'high' light intensity measurements? Normally this is 255, as in completely white, so effectively no mask.",
                $"--{Args.LcdHeight}\t\tThe height in pixels of the LCD screen of the printer.",
                $"--{Args.LcdMeasurementsFilePathHigh}\t\tThe file path of the CSV file containing the measurements with high light intensity.",
                $"--{Args.LcdMeasurementsFilePathLow}\t\tThe file path of the CSV file containing the measurements with low light intensity.",
                $"--{Args.LcdWidth}\t\tThe width in pixels of the LCD screen of the printer.",
                $"--{Args.LdrCalibrationFilePath}\t\tThe file path to the CSV file containing the measurements to the curve fit data of the LDR. The first column is the mask intensity, the second column is the resistance measured in ohm.",
                $"--{Args.Low}\t\tThe grey pixel value (0-255) of the mask used for the 'low' light intensity measurements.",
                $"--{Args.MaskFilePath}\t\tThe file path the mask should be saved to.",
                $"--{Args.MeasurementsNrOfColumns}\t\tThe number of evenly spread measurements you did on the X-axis. The number of columns, so to speak.",
                $"--{Args.MeasurementsNrOfRows}\t\tThe number of evenly spread measurements you did on the Y-axis. The number of rows, so to speak.",
                $"--{Args.OriginalExposureTime}\t\tThe time the resin is exposed to the UV light per layer, that currently yields successful prints."
            }.ToList();
            var argumentsParser = GetArgumentsParser();
            //Even when other arguments are set, when help is set, the help should be displayed.
            var arguments = new[] {"--lcdwidth", "800", helpArgument};
            var actualHelpTexts = new List<string>();
            argumentsParser.Output += (sender, helpText) =>
            {
                actualHelpTexts.Add(helpText);
            };

            //When
            argumentsParser.Parse(arguments, _supportedFileTypes);

            //Then
            actualHelpTexts.Should().BeEquivalentTo(expectedHelpTexts);
        }

        private static ArgumentsParser GetArgumentsParser()
        {
            return new ArgumentsParser();
        }
    }
}
