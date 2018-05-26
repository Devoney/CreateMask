using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using Octokit;

namespace CreateMask.Utilities
{
    public class ReleaseManager : IReleaseManager
    {
        private readonly IReleasesClient _releasesClient;
        private readonly GitHubRepoInfo _gitHubRepoInfo;

        public ReleaseManager(IReleasesClient releasesClient, GitHubRepoInfo gitHubRepoInfo)
        {
            _releasesClient = releasesClient;
            _gitHubRepoInfo = gitHubRepoInfo;
        }

        public async Task CheckForNewReleaseAsync(CheckForReleaseArgs checkForReleaseArgs)
        {
            IReadOnlyList<Release> releases;
            try
            {
                releases = await _releasesClient.GetAll(_gitHubRepoInfo.Owner, _gitHubRepoInfo.Name);
            }
            catch
            {
                return;
            }
            if (releases == null || releases.Count == 0) return;

            var latest = releases.First();
            var version = new Version(latest.TagName);
            if (version > checkForReleaseArgs.CurrentVersion)
            {
                var releaseInfo = new ReleaseInfo
                {
                    Version = version,
                    Uri = new Uri(latest.HtmlUrl)
                };
                checkForReleaseArgs.OnNewReleaseCallBack(releaseInfo);
            }
        }
    }
}
