using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Storage;
using CreateMask.Workers;
using Ninject;

namespace CreateMask.Main
{
    public static class KernelConstructor
    {
        public static IKernel GetKernel()
        {
            var kernel = new StandardKernel();
            Register(kernel);
            return kernel;
        }

        private static void Register(IKernel kernel)
        {
            kernel.Bind<IArgumentsParser>().To<ArgumentsParser>();
            kernel.Bind<IGenericGridLoader<int>>().To<GenericGridLoader<int>>();
            kernel.Bind<IGenericLoader<Measurement>>().To<GenericLoader<Measurement>>();
            kernel.Bind<IImageSaver>().To<ImageSaver>();
            kernel.Bind<IMaskIntensityResistanceInterpolator>().To<MaskIntensityResistanceInterpolator>();
            kernel.Bind<IMeasurementGridProcessor>().To<MeasurementGridProcessor>();
        }
    }
}
