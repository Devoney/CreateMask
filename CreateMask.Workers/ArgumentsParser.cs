using System;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Fclp;
using Args = CreateMask.Contracts.Constants.Arguments;

namespace CreateMask.Workers
{
    public class ArgumentsParser : IArgumentsParser
    {
        public ApplicationArguments Parse(string[] args)
        {
            var parser = new FluentCommandLineParser<ApplicationArgumentsWrapper>();

            parser.Setup(aa => aa.LcdHeight).As('h', Args.LcdHeight).Required();
            parser.Setup(aa => aa.LcdWidth).As('w', Args.LcdWidth).Required();
            parser.Setup(aa => aa.LdrCalibrationFilePath).As('l', Args.LdrCalibrationFilePath).Required();
            parser.Setup(aa => aa.LcdMeasurementsFilePathHigh).As('m', Args.LcdMeasurementsFilePathHigh).Required();
            parser.Setup(aa => aa.LcdMeasurementsFilePathLow).As('n', Args.LcdMeasurementsFilePathLow).Required();
            parser.Setup(aa => aa.MaskFilePath).As('o', Args.MaskFilePath).Required();
            parser.Setup(aa => aa.MeasurementsNrOfRows).As('r', Args.MeasurementsNrOfRows).Required();
            parser.Setup(aa => aa.MeasurementsNrOfColumns).As('c', Args.MeasurementsNrOfColumns).Required();
            parser.Setup(aa => aa.High).As('t', Args.High).SetDefault(byte.MaxValue);
            parser.Setup(aa => aa.Low).As('b', Args.Low).Required();
            parser.Setup(aa => aa.DesiredResistance).As('d', Args.DesiredResistance).Required();

            var parseResults = parser.Parse(args);
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
    }
}
