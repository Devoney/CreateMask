using System.Linq;
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

        private readonly string[] _issueLabels = {"bug", "error-report"};

        public GitHubIssueCreator(IIssuesClient issuesClient, GitHubRepoInfo gitHubRepoInfo)
        {
            _issuesClient = issuesClient;
            _gitHubRepoInfo = gitHubRepoInfo;
        }

        public void CreateIssue(ErrorReport errorReport)
        {
            var errorReportName = "Error Report " + errorReport.DateTime.ToString("yyyyMMddHHmmss");

            if (DoesIssueExist(errorReportName)) return;

            var newIssue = new NewIssue(errorReportName);
            foreach (var label in _issueLabels)
            {
                newIssue.Labels.Add(label);
            }
            
            var body = JsonConvert.SerializeObject(errorReport);
            newIssue.Body = body;

            var task = _issuesClient.Create(_gitHubRepoInfo.Owner, _gitHubRepoInfo.Name, newIssue);
            task.Wait();
        }

        private bool DoesIssueExist(string issueTitle)
        {
            var repositoryIssueRequest = new RepositoryIssueRequest
            {
                State = ItemStateFilter.All
            };
            foreach(var label in _issueLabels) repositoryIssueRequest.Labels.Add(label);
            var task = _issuesClient.GetAllForRepository(_gitHubRepoInfo.Owner, _gitHubRepoInfo.Name, repositoryIssueRequest);
            task.Wait();
            var issues = task.Result;
            if (issues == null) return false; //Not sure what happened, let's just upload the report.
            return issues.Any(i => i.Title == issueTitle);
        }
    }
}
