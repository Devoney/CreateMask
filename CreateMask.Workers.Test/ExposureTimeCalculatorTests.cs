using System;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Workers.Test
{
    [TestFixture]
    public class ExposureTimeCalculatorTests
    {
        [Test, Category(Categories.Unit)]
        public void ExposureTimeIsCorrectlyCalculated()
        {
            //Given
            const byte highMask = byte.MaxValue;
            const int originalExposureTime = 10000;
            const byte centerSpotMaskIntensity = 200;
            var expectedExposureTime = (int)(originalExposureTime*((double)highMask/centerSpotMaskIntensity));
            var maskIntensityGrid = new byte[,]
            {
                {240, 234, 232},
                {240, centerSpotMaskIntensity, 232},
                {240, 234, 232},
            };
            var tuple = GetExposureTimeCalculator(centerSpotMaskIntensity);
            var exposureTimeCalculator = tuple.Item1;
            var measurementGridProcessorMock = tuple.Item2;

            //When
            var actualExposureTime = exposureTimeCalculator.CalculateExposure(highMask, maskIntensityGrid, originalExposureTime);

            //Then
            actualExposureTime.Should().Be(expectedExposureTime);
            measurementGridProcessorMock.Verify(m => m.GetCenterMaskIntensity(maskIntensityGrid), Times.Once);
        }

        private static Tuple<IExposureTimeCalculator, Mock<IMeasurementGridProcessor>> GetExposureTimeCalculator(byte centerSpotMaskIntensity)
        {
            var measurementGridProcessorMock = new Mock<IMeasurementGridProcessor>();

            measurementGridProcessorMock
                .Setup(m => m.GetCenterMaskIntensity(It.IsAny<byte[,]>()))
                .Returns(centerSpotMaskIntensity);

            var exposureTimeCalculator = new ExposureTimeCalculator(measurementGridProcessorMock.Object);
            var tuple = new Tuple<IExposureTimeCalculator, Mock<IMeasurementGridProcessor>>(exposureTimeCalculator, measurementGridProcessorMock);
            return tuple;
        }
    }
}
