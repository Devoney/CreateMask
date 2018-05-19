using System;
using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IErrorReportCreator
    {
        void CreateReport(Version version, Exception exception, ApplicationArguments applicationArguments, string directory, string reportName);
    }
}
