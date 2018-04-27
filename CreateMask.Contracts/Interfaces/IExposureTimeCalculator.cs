using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IExposureTimeCalculator
    {
        int CalculateExposure(byte highMask, byte[,] maskGrid, int originalExposureTimeInSeconds);
    }
}
