using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Storage;
using CreateMask.Workers;
using Ninject;

namespace CreateMask.Main
{
    public static class KernelRegistrator
    {
        public static void Register(IKernel kernel)
        {
            kernel.Bind<IGenericGridLoader<int>>().To<GenericGridLoader<int>>();
            kernel.Bind<IGenericLoader<Measurement>>().To<GenericLoader<Measurement>>();
            kernel.Bind<IImageSaver>().To<ImageSaver>();
            kernel.Bind<IMaskIntensityResistanceInterpolator>().To<MaskIntensityResistanceInterpolator>();
            kernel.Bind<IMeasurementGridProcessor>().To<MeasurementGridProcessor>();
        }
    }
}
