using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using CreateMask.Storage;
using CreateMask.Utilities;
using CreateMask.Workers;
using CreateMask.Workers.Factories;
using Ninject;
using Octokit;
using Octokit.Internal;

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
            var gitHubRepoInfo = new GitHubRepoInfo("CreateMask", "Devoney", "create-mask-error-reporter", "createmask2018");
            kernel.Bind<GitHubRepoInfo>().ToConstant(gitHubRepoInfo);

            var errorReportConfiguration = new ErrorReportConfiguration("./error-reports", "./error-reports/reported");
            kernel.Bind<ErrorReportConfiguration>().ToConstant(errorReportConfiguration);

            kernel.Bind<IArgumentsParser>().To<ArgumentsParser>();
            kernel.Bind<IBitmapProcessor>().To<BitmapProcessor>();
            kernel.Bind<IDateTimeWorker>().To<DateTimeWorker>();
            kernel.Bind<ICloner>().To<Cloner>();
            kernel.Bind<IErrorReportCreator>().To<ErrorReportCreator>();
            kernel.Bind<IErrorReportReporter>().To<ErrorReportReporter>();
            kernel.Bind<IExposureTimeCalculator>().To <ExposureTimeCalculator>();
            kernel.Bind<IFileSystemWatcher>().To<FileSystemWatcher>();
            kernel.Bind<IGenericGridLoader<int>>().To<GenericGridLoader<int>>();
            kernel.Bind<IGenericLoader<Measurement>>().To<GenericLoader<Measurement>>();
            kernel.Bind<IGitHubIssueCreator>().To<GitHubIssueCreator>();
            kernel.Bind<IMaskIntensityResistanceInterpolatorFactory>().To<MaskIntensityResistanceInterpolatorFactory>();
            kernel.Bind<IMeasurementGridProcessor>().To<MeasurementGridProcessor>();
            kernel.Bind<IOutputWriter>().To<OutputWriter>();
            RegisterGithubClients(kernel);
            kernel.Bind<IReleaseManager>().To<ReleaseManager>();
            
        }

        private static void RegisterGithubClients(IKernel kernel)
        {
            var productHeaderValue = new ProductHeaderValue("CreateMask");
            var credentialStore = new InMemoryCredentialStore(new Credentials("create-mask-error-reporter", "createmask2018"));
            var githubClient = new GitHubClient(productHeaderValue, credentialStore);
            kernel.Bind<IReleasesClient>().ToConstant(githubClient.Repository.Release);
            kernel.Bind<IIssuesClient>().ToConstant(githubClient.Issue);
        }
    }
}
