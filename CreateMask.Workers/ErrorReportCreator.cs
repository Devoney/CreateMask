using System;
using CreateMask.Containers;

namespace CreateMask.Workers
{
    public class ErrorReportCreator
    {
        private readonly ErrorReport _errorReport = new ErrorReport();
        private string _errorReportsDirectory;

        public void CreateReport(Version version, Exception exception, ApplicationArguments applicationArguments, string errorReportsDirectory)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (applicationArguments == null) throw new ArgumentNullException(nameof(applicationArguments));
            if (errorReportsDirectory == null) throw new ArgumentNullException(nameof(errorReportsDirectory));

            _errorReportsDirectory = errorReportsDirectory;
            _errorReport.Version = version;
            _errorReport.Exception = exception;
            _errorReport.ApplicationArguments = applicationArguments;
        }
    }
}
