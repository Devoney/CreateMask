using System;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Newtonsoft.Json;
using Octokit;

namespace CreateMask.Workers
{
    public class GitHubIssueCreator : IGitHubIssueCreator
    {
        private readonly IIssuesClient _issuesClient;

        public GitHubIssueCreator(IIssuesClient issuesClient)
        {
            _issuesClient = issuesClient;
        }

        public void CreateIssue(ErrorReport errorReport)
        {
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 10);
            var newIssue = new NewIssue("Error Report " + errorReport.DateTime.ToString("yyyyMMddHHmmss"));
            newIssue.Labels.Add("bug");
            newIssue.Labels.Add("error-report");
            var body = JsonConvert.SerializeObject(errorReport);
            newIssue.Body = body;
            _issuesClient.Create("Devoney", "CreateMask", newIssue); // TODO: Get Devoney / CreateMask from central location
        }
    }
}
