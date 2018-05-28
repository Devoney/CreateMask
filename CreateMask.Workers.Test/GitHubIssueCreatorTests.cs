using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreateMask.Containers;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Octokit;
using TestHelpers;

namespace CreateMask.Workers.Test
{
    [TestFixture]
    public class GitHubIssueCreatorTests
    {
        [Test, Category(Categories.Unit)]
        public void IssueIsCreatedWithCorrectOwnerAndRepositoryName()
        {
            //Given
            var errorReport = new ErrorReport();
            var container = GetGitHubIssueCreator();
            var gitHubIssueCreator = container.GitHubIssueCreator;
            var issuesClientMock = container.IssuesClientMock;
            var owner = container.GitHubRepoInfo.Owner;
            var name = container.GitHubRepoInfo.Name;

            //When
            gitHubIssueCreator.CreateIssue(errorReport);

            //Then
            issuesClientMock.Verify(ic => ic.Create(owner, name, It.IsAny<NewIssue>()));
        }

        [Test, Category(Categories.Unit)]
        public void IssueHasCorrectTitleBodyAndLabels()
        {
            //Given
            var errorReport = new ErrorReport();
            var expectedJson = JsonConvert.SerializeObject(errorReport);
            var expectedLabels = new []{"error-report", "bug"};
            var expectedTitle = GetIssueTitle(errorReport);
            var container = GetGitHubIssueCreator();
            var gitHubIssueCreator = container.GitHubIssueCreator;

            //When
            gitHubIssueCreator.CreateIssue(errorReport);

            //Then
            var actualIssue = container.NewIssue;
            actualIssue.Should().NotBeNull();
            actualIssue.Title.Should().Be(expectedTitle);
            actualIssue.Labels.ToArray().Should().BeEquivalentTo(expectedLabels);
            actualIssue.Body.Should().Be(expectedJson);
        }

        [Test, Category(Categories.Unit)]
        public void IssueIsNotCreatedWhenTitleAlreadyExists()
        {
            //Given
            var errorReport = new ErrorReport();
            var container = GetGitHubIssueCreator();
            var owner = container.GitHubRepoInfo.Owner;
            var name = container.GitHubRepoInfo.Name;
            var repoIssueRequest = new RepositoryIssueRequest
            {
                State = ItemStateFilter.All,
                Labels = { "error-report", "bug"}
            };
            var gitHubIssueCreator = container.GitHubIssueCreator;
            var issue = new Issue();
            var titleProperty = typeof (Issue).GetProperty(nameof(Issue.Title));
            var issueTitle = GetIssueTitle(errorReport);
            titleProperty.SetValue(issue, issueTitle);
            var issueListToReturn = new List<Issue>();
            issueListToReturn.Add(issue);
            RepositoryIssueRequest actualRepositoryIssueRequest = null;
            container.IssuesClientMock
                .Setup(ic => ic.GetAllForRepository(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RepositoryIssueRequest>()))
                .Callback(new Action<string, string, RepositoryIssueRequest>((o, n, repoIssueReq) =>
                {
                    actualRepositoryIssueRequest = repoIssueReq;
                }))
                .Returns(Task.Run(new Func<IReadOnlyList<Issue>>(() => issueListToReturn)));

            //When
            gitHubIssueCreator.CreateIssue(errorReport);

            //Then
            container.IssuesClientMock.Verify(ic => ic.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewIssue>()), Times.Never);
            container.IssuesClientMock.Verify(ic => ic.GetAllForRepository(owner, name, It.IsAny<RepositoryIssueRequest>()), Times.Once);
            actualRepositoryIssueRequest.Should().BeEquivalentTo(repoIssueRequest);
        }

        #region Helpers
        private static string GetIssueTitle(ErrorReport errorReport)
        {
            return "Error Report " + errorReport.DateTime.ToString("yyyyMMddHHmmss");
        }

        private static Container GetGitHubIssueCreator()
        {
            var gitHubRepoInfo = new GitHubRepoInfo("CreateMask", "Devoney", "username", "password");
            var issuesClientMock = new Mock<IIssuesClient>();
            var gitHubIssueCreator = new GitHubIssueCreator(issuesClientMock.Object, gitHubRepoInfo);
            var container = new Container(gitHubIssueCreator, issuesClientMock, gitHubRepoInfo);

            issuesClientMock
                .Setup(ic => ic.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NewIssue>()))
                .Callback(new Action<string, string, NewIssue>((argOwner, argName, newIssue) =>
                {
                    container.NewIssue = newIssue;
                }))
                .Returns(Task.Run(new Func<Issue>(() => null)));

            return container;
        }

        private class Container
        {
            public GitHubIssueCreator GitHubIssueCreator { get; private set; }
            public Mock<IIssuesClient> IssuesClientMock { get; private set; }
            public GitHubRepoInfo GitHubRepoInfo { get; private set; }
            public NewIssue NewIssue { get; set; }

            public Container(GitHubIssueCreator gitHubIssueCreator, Mock<IIssuesClient> issuesClientMock, GitHubRepoInfo gitHubRepoInfo)
            {
                GitHubIssueCreator = gitHubIssueCreator;
                IssuesClientMock = issuesClientMock;
                GitHubRepoInfo = gitHubRepoInfo;
            }
        }
        #endregion
    }
}
