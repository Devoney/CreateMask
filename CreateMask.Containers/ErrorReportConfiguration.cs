namespace CreateMask.Containers
{
    public class ErrorReportConfiguration
    {
        public string MainDirectory { get; private set; }
        public string ReportedDirectory { get; private set; }

        public ErrorReportConfiguration(string mainDirectory, string reportedDirectory)
        {
            MainDirectory = mainDirectory;
            ReportedDirectory = reportedDirectory;
        }
    }
}
