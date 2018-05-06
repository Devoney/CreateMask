using System;
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

        public ReleaseManager(IReleasesClient releasesClient)
        {
            _releasesClient = releasesClient;
        }

        public async Task CheckForNewReleaseAsync(CheckForReleaseInfo checkForReleaseInfo)
        {
            var releases = await _releasesClient.GetAll(checkForReleaseInfo.Owner, checkForReleaseInfo.Repository);
            if (releases == null || releases.Count == 0) return;

            var latest = releases.First();
            var version = new Version(latest.TagName);
            if (version > checkForReleaseInfo.CurrentVersion)
            {
                var releaseInfo = new ReleaseInfo
                {
                    Version = version,
                    Uri = new Uri(latest.Url)
                };
                checkForReleaseInfo.OnNewReleaseCallBack(releaseInfo);
            }
        }
    }
}
