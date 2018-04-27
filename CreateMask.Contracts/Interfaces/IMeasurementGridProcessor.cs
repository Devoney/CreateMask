using System.Drawing;
using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IMeasurementGridProcessor
    {
        MinMax<Measurement>[,] CreateMinMaxMeasurementGrid(byte low, byte high,
            int[,] resistanceMeasurementsLow,
            int[,] resistanceMeasurementsHigh);

        byte[,] CreateLocalMaskIntensityGrid(IMaskIntensityResistanceInterpolator maskIntensityInterpolator, MinMax<Measurement>[,] minMaxResistanceGrid, int desiredResistance);
        Bitmap CreateBitMap(byte[,] localMaskIntensityGrid);
        double GetCenterMaskIntensity(byte[,] measurementGrid);
    }
}