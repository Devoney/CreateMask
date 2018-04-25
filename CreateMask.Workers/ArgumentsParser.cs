using System;
using System.Collections.Generic;
using System.Linq;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Fclp;
using Args = CreateMask.Contracts.Constants.Arguments;

namespace CreateMask.Workers
{
    public class ArgumentsParser : IArgumentsParser
    {
        public event EventHandler<string> Output;
        
        public ApplicationArguments Parse(string[] args, IEnumerable<string> supportedFileTypes)
        {
            var parser = new FluentCommandLineParser<ApplicationArgumentsWrapper>();

            parser.Setup(aa => aa.FileType)
                .As('a', Args.FileType)
                .WithDescription("The type of file to output. Supported file types are: " + string.Join(", ", supportedFileTypes));

            parser.Setup(aa => aa.LcdHeight)
                .As('h', Args.LcdHeight).Required()
                .WithDescription("The height in pixels of the LCD screen of the printer.");

            parser.Setup(aa => aa.LcdWidth)
                .As('w', Args.LcdWidth).Required()
                .WithDescription("The width in pixels of the LCD screen of the printer.");

            parser.Setup(aa => aa.LdrCalibrationFilePath)
                .As('l', Args.LdrCalibrationFilePath).Required()
                .WithDescription("The file path to the CSV file containing the measurements " +
                    "to the curve fit data of the LDR. The first column is the mask " +
                    "intensity, the second column is the resistance measured in ohm.");

            parser.Setup(aa => aa.LcdMeasurementsFilePathHigh)
                .As('m', Args.LcdMeasurementsFilePathHigh).Required()
                .WithDescription("The file path of the CSV file containing the measurements with high light intensity.");

            parser.Setup(aa => aa.LcdMeasurementsFilePathLow)
                .As('n', Args.LcdMeasurementsFilePathLow).Required()
                .WithDescription("The file path of the CSV file containing the measurements with low light intensity.");

            parser.Setup(aa => aa.MaskFilePath)
                .As('o', Args.MaskFilePath).Required()
                .WithDescription("The file path the mask should be saved to.");

            parser.Setup(aa => aa.MeasurementsNrOfRows)
                .As('r', Args.MeasurementsNrOfRows).Required()
                .WithDescription("The number of evenly spread measurements you did on the Y-axis. The number of rows, so to speak.");

            parser.Setup(aa => aa.MeasurementsNrOfColumns)
                .As('c', Args.MeasurementsNrOfColumns).Required()
                .WithDescription("The number of evenly spread measurements you did on the X-axis. The number of columns, so to speak.");

            parser.Setup(aa => aa.High)
                .As('t', Args.High).SetDefault(byte.MaxValue)
                .WithDescription("[OPTIONAL, DEFAULT=255] The pixel value(0-255) of the mask used for the 'high' light intensity measurements? Normally this is 255, as in completely white, so effectively no mask.") ;

            parser.Setup(aa => aa.Low)
                .As('b', Args.Low).Required()
                .WithDescription("The grey pixel value (0-255) of the mask used for the 'low' light intensity measurements.");

            parser.Setup(aa => aa.DesiredResistance)
                .As('d', Args.DesiredResistance).Required()
                .WithDescription("The resistance value you want to normalize the entire LCD screen to. This determines the increase in exposure time.");

            var parseResults = parser.Parse(args);

            if (args == null || args.Length == 0 || args.Intersect(new[] {"?", "help", "-help", "--help"}).Any())
            {
                foreach (var option in parser.Options.OrderBy(o => o.LongName))
                {
                    OnOutput($"--{option.LongName}\t\t{option.Description}");
                }
                return null;
            }

            if (parseResults.HasErrors)
            {
                throw new ArgumentException(parseResults.ErrorText);
            }

            return parser.Object;
        }

        private class ApplicationArgumentsWrapper : ApplicationArguments
        {
            public new int Low
            {
                get { return base.Low; }
                set
                {
                    CheckValueToBeInByteRange(value, Args.Low);
                    base.Low = (byte)value;
                }
            }

            public new int High
            {
                get { return base.High; }
                set
                {
                    CheckValueToBeInByteRange(value, Args.High);
                    base.High = (byte)value;
                }
            }

            private void CheckValueToBeInByteRange(int input, string option)
            {
                if (input > byte.MaxValue || input < byte.MinValue)
                {
                    throw new InvalidOperationException($"Value of '{input}' should be within range of {byte.MinValue} to {byte.MaxValue} for option '{option}'.");
                }
            }
        }

        private void OnOutput(string e)
        {
            Output?.Invoke(this, e);
        }
    }
}
