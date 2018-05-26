using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Newtonsoft.Json;
using Octokit;

namespace CreateMask.Workers
{
    public class GitHubIssueCreator : IGitHubIssueCreator
    {
        private readonly IIssuesClient _issuesClient;
        private readonly GitHubRepoInfo _gitHubRepoInfo;

        public GitHubIssueCreator(IIssuesClient issuesClient, GitHubRepoInfo gitHubRepoInfo)
        {
            _issuesClient = issuesClient;
            _gitHubRepoInfo = gitHubRepoInfo;
        }

        public void CreateIssue(ErrorReport errorReport)
        {
            var newIssue = new NewIssue("Error Report " + errorReport.DateTime.ToString("yyyyMMddHHmmss"));
            newIssue.Labels.Add("bug");
            newIssue.Labels.Add("error-report");
            var body = JsonConvert.SerializeObject(errorReport);
            newIssue.Body = body;
            _issuesClient.Create(_gitHubRepoInfo.Owner, _gitHubRepoInfo.Owner, newIssue);
        }
    }
}
