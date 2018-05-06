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
            var items = CallBackIsCalledWhenNewReleaseIsAvailable_Given(info => { callbackCalled = true; });
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
            var items = CallBackIsCalledWhenNewReleaseIsAvailable_Given(info => { callbackCalled = true; }, new Version(1,0,0,0));
            var releaseManager = items.Item1;
            var arguments = items.Item2;
            var releasesClientMock = items.Item3;

            //When
            await releaseManager.CheckForNewReleaseAsync(arguments).ConfigureAwait(false);

            //Then
            releasesClientMock.Verify(m => m.GetAll(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "It is expected that the releases are queried by the client.");
            callbackCalled.Should().BeFalse();
        }

        private Tuple<IReleaseManager, CheckForReleaseInfo, Mock<IReleasesClient>> 
            CallBackIsCalledWhenNewReleaseIsAvailable_Given(Action<ReleaseInfo> callback, Version latestReleaseVersion = null)
        {
            latestReleaseVersion = latestReleaseVersion ?? new Version(1, 0, 0, 1);
            var releasesClientMock = new Mock<IReleasesClient>();
            releasesClientMock.Setup(client => client.GetAll(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() =>
                {
                    var release = CreateRelease(latestReleaseVersion);
                    var list = new List<Release> { release };
                    var task = Task.Run(() => (IReadOnlyList<Release>)list);
                    task.ConfigureAwait(false);
                    return task;
                });

            var releasesClient = releasesClientMock.Object;
            var releaseManager = new ReleaseManager(releasesClient);
            var arguments = new CheckForReleaseInfo
            {
                CurrentVersion = new Version(1, 0, 0, 0),
                Repository = "MyRepo",
                Owner = "SomeOwner",
                OnNewReleaseCallBack = callback
            };
            return new Tuple<IReleaseManager, CheckForReleaseInfo, Mock<IReleasesClient>>(releaseManager, arguments, releasesClientMock);
        }

        private static Release CreateRelease(Version version)
        {
            return new Release("http://www.github.com/Devoney/CreateMask/Releases/" + version, "htmlurl", "assetsurl", "uploadurl", 
                1, version.ToString(), "targetCommitish", "MyName", 
                "body", false, false, DateTimeOffset.MaxValue, 
                DateTimeOffset.MinValue, null, "tarballurl", 
                "zipballurl", null);
        }
    }
}
