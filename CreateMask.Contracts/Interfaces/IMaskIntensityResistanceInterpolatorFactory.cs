using System.Collections.Generic;
using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IMaskIntensityResistanceInterpolatorFactory
    {
        IMaskIntensityResistanceInterpolator Create(IEnumerable<Measurement> measurements);
    }
}
