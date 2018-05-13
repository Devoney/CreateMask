using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Storage;
using CreateMask.Utilities;
using CreateMask.Workers;
using CreateMask.Workers.Factories;
using Ninject;
using Octokit;

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
            kernel.Bind<IErrorReportCreator>().To<ErrorReportCreator>();
            kernel.Bind<IExposureTimeCalculator>().To <ExposureTimeCalculator> ();
            kernel.Bind<IGenericGridLoader<int>>().To<GenericGridLoader<int>>();
            kernel.Bind<IGenericLoader<Measurement>>().To<GenericLoader<Measurement>>();
            kernel.Bind<IMaskIntensityResistanceInterpolatorFactory>().To<MaskIntensityResistanceInterpolatorFactory>();
            kernel.Bind<IMeasurementGridProcessor>().To<MeasurementGridProcessor>();
            RegisterGithubReleasesClient(kernel);
            kernel.Bind<IReleaseManager>().To<ReleaseManager>();
            
        }

        private static void RegisterGithubReleasesClient(IKernel kernel)
        {
            var productHeaderValue = new ProductHeaderValue("CreateMask");
            var githubClient = new GitHubClient(productHeaderValue);
            var releasesClient = githubClient.Repository.Release;
            kernel.Bind<IReleasesClient>().ToConstant(releasesClient);
        }
    }
}
