using System;
using CreateMask.Contracts.Interfaces;

namespace CreateMask.Workers
{
    public class ExposureTimeCalculator : IExposureTimeCalculator
    {
        private readonly IMeasurementGridProcessor _measurementGridProcessor;

        public ExposureTimeCalculator(IMeasurementGridProcessor measurementGridProcessor)
        {
            _measurementGridProcessor = measurementGridProcessor;
        }

        public int CalculateExposure(byte highMask, byte[,] maskGrid, int originalExposureTimeInSeconds)
        {
            var centerMaskIntensity = _measurementGridProcessor.GetCenterMaskIntensity(maskGrid);

            var ratio = highMask/centerMaskIntensity;
            return (int)Math.Round(originalExposureTimeInSeconds*ratio);
        }
    }
}
