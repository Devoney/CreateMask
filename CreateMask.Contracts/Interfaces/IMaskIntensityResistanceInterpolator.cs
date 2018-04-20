using System.Collections.Generic;
using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IMaskIntensityResistanceInterpolator
    {
        byte GetMaskIntensity(int resistance);
        int GetResistance(byte maskIntensity);
        void LoadMeasurements(List<Measurement> measurements);
        byte GetLocalMaskIntensity(int desiredLocalResistance, MinMax<Measurement> localMeasurement);
    }
}
