using System.Drawing;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Workers.Test
{
    [TestFixture]
    public class MeasurementGridProcessorTests
    {
        [Test, Category(Categories.Unit)]
        public void CorrectMinMaxMeasurementGridIsConstructed()
        {
            //Given
            const byte lowestMaskIntensity = 127;
            const byte highestMaskIntensity = 255;
            var gridLow = GetGridLow();
            var gridHigh = GetGridHigh();
            var minMaxMaskIntensity = new MinMax<byte>(lowestMaskIntensity, highestMaskIntensity);
            var expectedMinMasMeasurementGrid = CreateMinMaxMeasurementGrid(minMaxMaskIntensity, gridLow, gridHigh);
            var mgp = GetMeasurementGridProcessor();

            //When
            var actualMinMaxMeasurementGrid = mgp.CreateMinMaxMeasurementGrid(lowestMaskIntensity, highestMaskIntensity, gridLow, gridHigh);

            //Then
            actualMinMaxMeasurementGrid.Should().BeEquivalentTo(expectedMinMasMeasurementGrid);
        }

        [Test, Category(Categories.Unit)]
        public void CorrecLocalMaskIntensityGridIsConstructed()
        {
            //Given
            const int desiredResistance = 8;
            const byte mockedMaskIntensity = 240;
            const int toLowLightIntensityToAdjustResistance = 16;
            var mgp = GetMeasurementGridProcessor();
            var minMaxMaskIntensity = new MinMax<byte>(127, 255);
            var gridLow = GetGridLow();
            var gridHigh = GetGridHigh();
            gridHigh[0, 0] = toLowLightIntensityToAdjustResistance;
            gridHigh[2, 3] = toLowLightIntensityToAdjustResistance;
            var expectedIntensityGrid = new [,]
            {
                {byte.MaxValue, mockedMaskIntensity, mockedMaskIntensity, mockedMaskIntensity},
                {mockedMaskIntensity, mockedMaskIntensity, mockedMaskIntensity, mockedMaskIntensity},
                {mockedMaskIntensity, mockedMaskIntensity, mockedMaskIntensity, byte.MaxValue}
            };
            var minMaxMeasurementGrid = CreateMinMaxMeasurementGrid(minMaxMaskIntensity, gridLow, gridHigh);
            var maskIntensityResistanceInterPolatorMock = new Mock<IMaskIntensityResistanceInterpolator>();
            maskIntensityResistanceInterPolatorMock
                .Setup(m => m.GetLocalMaskIntensity(It.IsAny<int>(), It.IsAny<MinMax<Measurement>>()))
                .Returns(mockedMaskIntensity);
            var maskIntensityResistanceInterPolator = maskIntensityResistanceInterPolatorMock.Object;

            //When
            var actualIntensityGrid = mgp.CreateLocalMaskIntensityGrid(maskIntensityResistanceInterPolator, minMaxMeasurementGrid, desiredResistance);

            //Then
            actualIntensityGrid.Should().BeEquivalentTo(expectedIntensityGrid);
            maskIntensityResistanceInterPolatorMock.Verify(m => m.GetLocalMaskIntensity(
                It.IsAny<int>(), 
                It.IsAny<MinMax<Measurement>>()),
                Times.Exactly(10));
        }

        [Test, Category(Categories.Unit)]
        public void CorrectBitmapIsConstructedFromMaskIntensityGrid()
        {
            //Given
            var intensityGrid = new byte[,]
            {
                {4,5,6 },
                {7,8,9 }
            };
            var bitMapSize = new Size(intensityGrid.GetLength(1), intensityGrid.GetLength(0));
            var expectedBitMap = new Bitmap(bitMapSize.Width, bitMapSize.Height);
            for (var r = 0; r < intensityGrid.GetLength(0); r++)
            {
                for (var c = 0; c < intensityGrid.GetLength(1); c++)
                {
                    var cc = intensityGrid[r, c];
                    var color = Color.FromArgb(cc, cc, cc);
                    expectedBitMap.SetPixel(c, r, color);
                }
            }
            var mgp = GetMeasurementGridProcessor();

            //When
            var actualBitmap = mgp.CreateBitMap(intensityGrid);

            //Then
            AssertExt.Equals(expectedBitMap, actualBitmap);
        }

        [Test, Category(Categories.Unit)]
        public void CenterMaskIntensityIsReturnedAsAverage()
        {
            //Given
            var maskItensity2DArray = new byte[,]
            {
                { 1,2,3,4,5, 6},
                {7,8,9,10,11,12 },
                {13,14,15,16,17,18},
                {19,20,21,22,23,24 }
            };
            const double expectedMaskIntensity = 12.5;
            var measurementGridProcessor = GetMeasurementGridProcessor();

            //When
            var actualCenterMeasurement = measurementGridProcessor.GetCenterMaskIntensity(maskItensity2DArray);

            //Then
            actualCenterMeasurement.Should().Be(expectedMaskIntensity);
        }

        #region Helpers
        private int[,] GetGridHigh()
        {
            return new[,]
            {
                {1,2,3,4 },
                {4,3,2,1 },
                {5,6,7,8 }
            };
        }

        private int[,] GetGridLow()
        {
            return new[,]
            {
                {10,20,30,40 },
                {40,30,20,10 },
                {50,60,70,80 }
            };
        }

        private MinMax<Measurement>[,] CreateMinMaxMeasurementGrid(MinMax<byte> maskIntensity, int[,] gridLow, int[,] gridHigh)
        {
            var nrOfRows = gridLow.GetLength(0);
            var nrOfColumns = gridLow.GetLength(1);

            var minMasMeasurementGrid = new MinMax<Measurement>[nrOfRows, nrOfColumns];
            for (var r = 0; r < nrOfRows; r++)
            {
                for (var c = 0; c < nrOfColumns; c++)
                {
                    var low = gridLow[r, c];
                    var high = gridHigh[r, c];
                    var measurementMin = new Measurement(maskIntensity.Min, low);
                    var measurementMax = new Measurement(maskIntensity.Max, high);
                    var minMaxMeasurement = new MinMax<Measurement>(measurementMin, measurementMax);
                    minMasMeasurementGrid[r, c] = minMaxMeasurement;
                }
            }

            return minMasMeasurementGrid;
        }

        private IMeasurementGridProcessor GetMeasurementGridProcessor()
        {
            return new MeasurementGridProcessor();
        }
        #endregion
    }
}
