using System;
using System.Drawing;
using System.Linq;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Utilities;
using CreateMask.Workers.Exceptions;

namespace CreateMask.Workers
{
    public class MeasurementGridProcessor : IMeasurementGridProcessor
    {
        public MinMax<Measurement>[,] CreateMinMaxMeasurementGrid(byte low, byte high,
            int[,] resistanceMeasurementsLow,
            int[,] resistanceMeasurementsHigh)
        {
            var nrOfRowsLow = resistanceMeasurementsLow.GetLength(0);
            var nrOfColumnsLow = resistanceMeasurementsLow.GetLength(1);

            var nrOfRowsHigh = resistanceMeasurementsHigh.GetLength(0);
            var nrOfColumnsHigh = resistanceMeasurementsHigh.GetLength(1);

            if (nrOfRowsLow != nrOfRowsHigh || nrOfColumnsLow != nrOfColumnsHigh)
            {
                throw new InvalidOperationException("The dimensions of the given resistance 2D arrays do not match.");
            }

            AssureLowMeasurementsAreLowerThanHighMeasurements(resistanceMeasurementsLow, resistanceMeasurementsHigh);

            var minMaxMeasurementsGrid = new MinMax<Measurement>[nrOfRowsLow, nrOfColumnsLow];

            for (var r = 0; r < nrOfRowsLow; r++)
            {
                for (var c = 0; c < nrOfColumnsLow; c++)
                {
                    var resistanceHigh = resistanceMeasurementsHigh[r, c];
                    var measurementHigh = new Measurement(high, resistanceHigh);

                    var resistanceLow = resistanceMeasurementsLow[r, c];
                    var measurementLow = new Measurement(low, resistanceLow);

                    var minMax = new MinMax<Measurement>(measurementLow, measurementHigh);
                    minMaxMeasurementsGrid[r, c] = minMax;
                }
            }

            return minMaxMeasurementsGrid;
        }

        public byte[,] CreateLocalMaskIntensityGrid(IMaskIntensityResistanceInterpolator maskIntensityInterpolator, MinMax<Measurement>[,] minMaxResistanceGrid, int desiredResistance)
        {
            var nrOfRows = minMaxResistanceGrid.GetLength(0);
            var nrOfColumns = minMaxResistanceGrid.GetLength(1);

            var localMaskIntensityGrid = new byte[nrOfRows, nrOfColumns];

            for (var r = 0; r < nrOfRows; r++)
            {
                for (var c = 0; c < nrOfColumns; c++)
                {
                    var minMaxMeasurement = minMaxResistanceGrid[r, c];
                    if (minMaxMeasurement.Max.Resistance > desiredResistance)
                    {
                        //We can not brighten this spot, it is dimmer than the desired light intensity.
                        localMaskIntensityGrid[r, c] = byte.MaxValue;
                    }
                    else
                    {
                        localMaskIntensityGrid[r, c] = maskIntensityInterpolator.GetLocalMaskIntensity(desiredResistance, minMaxMeasurement);
                    }
                }
            }

            return localMaskIntensityGrid;
        }

        public Bitmap CreateBitMap(byte[,] localMaskIntensityGrid)
        {
            var nrOfRows = localMaskIntensityGrid.GetLength(0);
            var nrOfColumns = localMaskIntensityGrid.GetLength(1);

            var bitmap = new Bitmap(nrOfColumns, nrOfRows);

            for (var r = 0; r < nrOfRows; r++)
            {
                for (var c = 0; c < nrOfColumns; c++)
                {
                    var i = localMaskIntensityGrid[r, c];
                    var color = Color.FromArgb(byte.MaxValue, i, i, i);
                    bitmap.SetPixel(c, r, color);
                }
            }

            return bitmap;
        }

        public double GetCenterMaskIntensity(byte[,] maskIntensityGrid)
        {
            var centerItems = maskIntensityGrid.GetCenterItems().ToList();
            return centerItems.Average(b => (double)b);
        }

        private void AssureLowMeasurementsAreLowerThanHighMeasurements(
            int[,] resistanceMeasurementsLow,
            int[,] resistanceMeasurementsHigh)
        {
            var rows = resistanceMeasurementsLow.GetLength(0);
            var columns = resistanceMeasurementsLow.GetLength(1);

            var lowsHigherOrEqualThanHigh = 0;
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < columns; c++)
                {
                    var lowMeasurement = resistanceMeasurementsLow[r, c];
                    var highMeasurement = resistanceMeasurementsHigh[r, c];
                    if (lowMeasurement >= highMeasurement) lowsHigherOrEqualThanHigh++;
                }
            }

            var nrOfMeasurements = rows*columns;
            if (lowsHigherOrEqualThanHigh == nrOfMeasurements)
            {
                throw new LowHighMeasurementsSwappedException("All measurements from low are equal or higher than the high measurements.");
            }
            throw new LowHigherThanHighMeasurementException($"{lowsHigherOrEqualThanHigh} out of " +
                                                            $"{nrOfMeasurements} measurements from " +
                                                            $"low where equal or higher than the high measurement.");
            
        }
    }
}
