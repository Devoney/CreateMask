using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
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

        private readonly List<string> SupportedFileTypes = new List<string>
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
                                               $"--{Args.FileType} {FileType}";

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
                FileType = FileType
            };
            var argumentsParser = GetArgumentsParser();

            //When
            var actualArguments = argumentsParser.Parse(_args, SupportedFileTypes);

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
                "--desiredresistance\t\tThe resistance value you want to normalize the entire LCD screen to. This determines the increase in exposure time.",
                "--filetype\t\tThe type of file to output. Supported file types are: Bmp, Png",
                "--high\t\t[OPTIONAL, DEFAULT=255] The pixel value(0-255) of the mask used for the 'high' light intensity measurements? Normally this is 255, as in completely white, so effectively no mask.",
                "--lcdheight\t\tThe height in pixels of the LCD screen of the printer.",
                "--lcdmeasurementsfile_high\t\tThe file path of the CSV file containing the measurements with high light intensity.",
                "--lcdmeasurementsfile_low\t\tThe file path of the CSV file containing the measurements with low light intensity.",
                "--lcdwidth\t\tThe width in pixels of the LCD screen of the printer.",
                "--ldrcalfile\t\tThe file path to the CSV file containing the measurements to the curve fit data of the LDR. The first column is the mask intensity, the second column is the resistance measured in ohm.",
                "--low\t\tThe grey pixel value (0-255) of the mask used for the 'low' light intensity measurements.",
                "--maskfilepath\t\tThe file path the mask should be saved to.",
                "--nrofcolumns\t\tThe number of evenly spread measurements you did on the X-axis. The number of columns, so to speak.",
                "--nrofrows\t\tThe number of evenly spread measurements you did on the Y-axis. The number of rows, so to speak.",
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
            argumentsParser.Parse(arguments, SupportedFileTypes);

            //Then
            actualHelpTexts.Should().BeEquivalentTo(expectedHelpTexts);
        }

        private static ArgumentsParser GetArgumentsParser()
        {
            return new ArgumentsParser();
        }
    }
}
