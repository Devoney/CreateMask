using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Octokit;
using TestHelpers;

namespace CreateMask.Utilities.Test
{
    [TestFixture]
    public class ReleaseManagerTests
    {
        [Test, Category(Categories.Unit)]
        public async void CallBackIsCalledWhenNewReleaseIsAvailable()
        {
            //Given
            var callbackCalled = false;
            var items = Given(info => { callbackCalled = true; }, new Version(1,0,0,1));
            var releaseManager = items.Item1;
            var arguments = items.Item2;
            var releasesClientMock = items.Item3;

            //When
            await releaseManager.CheckForNewReleaseAsync(arguments).ConfigureAwait(false);

            //Then
            releasesClientMock.Verify(m => m.GetAll(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "It is expected that the releases are queried by the client.");
            callbackCalled.Should().BeTrue();
        }

        [Test, Category(Categories.Unit)]
        public async void CallBackIsNotCalledWhenNoNewReleaseIsAvailable()
        {
            //Given
            var callbackCalled = false;
            var items = Given(info => { callbackCalled = true; }, new Version(1,0,0,0));
            var releaseManager = items.Item1;
            var arguments = items.Item2;
            var releasesClientMock = items.Item3;

            //When
            await releaseManager.CheckForNewReleaseAsync(arguments).ConfigureAwait(false);

            //Then
            releasesClientMock.Verify(m => m.GetAll(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "It is expected that the releases are queried by the client.");
            callbackCalled.Should().BeFalse();
        }

        [Test, Category(Categories.Unit)]
        public async void EmptyReleaseListResultsInNoCallback()
        {
            //Given
            var callbackCalled = false;
            var items = Given(info => { callbackCalled = true; }, null);
            var releaseManager = items.Item1;
            var arguments = items.Item2;
            var releasesClientMock = items.Item3;

            //When
            await releaseManager.CheckForNewReleaseAsync(arguments).ConfigureAwait(false);

            //Then
            releasesClientMock.Verify(m => m.GetAll(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "It is expected that the releases are queried by the client.");
            callbackCalled.Should().BeFalse();
        }

        [Test, Category(Categories.Unit), Description("When fetching the releases fail, " +
                                                      "the chance is very high that its just a network issue of some kind.")]
        public async void ExceptionInReleasesClientResultsInCallbackNotCalled()
        {
            //Given
            var tuple = GetReleaseManager();
            var releasesClientMock = tuple.Item1;
            var releaseManager = tuple.Item2;
            var callbackCalled = false;
            releasesClientMock.Setup(client => client.GetAll(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new TimeoutException("The network operation timed out"));
            var arguments = new CheckForReleaseArgs
            {
                CurrentVersion = new Version(),
                OnNewReleaseCallBack = releaseInfo =>
                {
                    callbackCalled = true;
                }
            };

            //When
            await releaseManager.CheckForNewReleaseAsync(arguments).ConfigureAwait(false);

            //Then
            releasesClientMock.Verify(m => m.GetAll(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "It is expected that the releases are queried by the client.");
            callbackCalled.Should().BeFalse();
        }

        [Test, Category(Categories.Unit)]
        public async void CorrectUrlAndVersionIsReturnedWhenNewVersionIsAvailable()
        {
            //Given
            var latestVersion = new Version(1, 0, 0, 3);
            var expectedUrl = "http://www.github.com/Devoney/CreateMask/Releases/" + latestVersion;
            ReleaseInfo releaseInfo = null;
            var items = Given(info =>
            {
                releaseInfo = info;
            }, latestVersion);
            var releaseManager = items.Item1;
            var args = items.Item2;
            var releasesClientMock = items.Item3;

            //When
            await releaseManager.CheckForNewReleaseAsync(args);

            //Then
            releasesClientMock.Verify(m => m.GetAll(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "It is expected that the releases are queried by the client.");
            releaseInfo.Should().NotBeNull();
            releaseInfo.Version.Should().BeEquivalentTo(latestVersion);
            releaseInfo.Uri.ToString().Should().Be(expectedUrl);
        }

        private Tuple<Mock<IReleasesClient>, IReleaseManager> GetReleaseManager()
        {
            var releasesClient = new Mock<IReleasesClient>();
            var gitHubRepoInfo = new GitHubRepoInfo("CreateMask", "Devoney", "Username", "Password");
            var releaseManager = new ReleaseManager(releasesClient.Object, gitHubRepoInfo);
            return new Tuple<Mock<IReleasesClient>, IReleaseManager>(releasesClient, releaseManager);
        }

        private Tuple<IReleaseManager, CheckForReleaseArgs, Mock<IReleasesClient>> 
            Given(Action<ReleaseInfo> callback, Version latestReleaseVersion)
        {
            var tuple = GetReleaseManager();
            var releasesClientMock = tuple.Item1;
            releasesClientMock.Setup(client => client.GetAll(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() =>
                {
                    var list = new List<Release>();
                    if (latestReleaseVersion != null)
                    {
                        var release = CreateRelease(latestReleaseVersion);
                        list.Add(release);
                    }
                    var task = Task.Run(() => (IReadOnlyList<Release>)list);
                    task.ConfigureAwait(false);
                    return task;
                });
            var releaseManager = tuple.Item2;
            
            var arguments = new CheckForReleaseArgs
            {
                CurrentVersion = new Version(1, 0, 0, 0),
                OnNewReleaseCallBack = callback
            };
            return new Tuple<IReleaseManager, CheckForReleaseArgs, Mock<IReleasesClient>>(releaseManager, arguments, releasesClientMock);
        }

        private static Release CreateRelease(Version version)
        {
            return new Release("url", "http://www.github.com/Devoney/CreateMask/Releases/" + version, "assetsurl", "uploadurl", 
                1, version.ToString(), "targetCommitish", "MyName", 
                "body", false, false, DateTimeOffset.MaxValue, 
                DateTimeOffset.MinValue, null, "tarballurl", 
                "zipballurl", null);
        }
    }
}
