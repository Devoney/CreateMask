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
                                     $"--{Args.DesiredResistance} {DesiredResistance}";

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
                DesiredResistance = DesiredResistance
            };
            var argumentsParser = GetArgumentsParser();

            //When
            var actualArguments = argumentsParser.Parse(_args);

            //Then
            actualArguments.Should().BeEquivalentTo(expectedArguments);
        }

        private IArgumentsParser GetArgumentsParser()
        {
            return new ArgumentsParser();
        }
    }
}
