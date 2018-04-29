using System.Collections.Generic;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;

namespace CreateMask.Workers.Factories
{
    public class MaskIntensityResistanceInterpolatorFactory : IMaskIntensityResistanceInterpolatorFactory
    {
        public IMaskIntensityResistanceInterpolator Create(IEnumerable<Measurement> measurements)
        {
            return new MaskIntensityResistanceInterpolator(measurements);
        }
    }
}
