using System.IO;
using System.Threading;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Utilities.Test
{
    [TestFixture]
    public class FileSytsemWatcherTests
    {
        [Test, Category(Categories.Unit)]
        public void NewFileIsNoticed()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                var filePath = Path.GetFullPath(Path.Combine(directory, "newfile"));
                var fsw = GetFileSystemWatcher();
                FileSystemEventArgs fileSystemEventArgs = null;
                fsw.Created += (sender, args) =>
                {
                    fileSystemEventArgs = args;
                };
                fsw.Start(directory);

                //When
                File.WriteAllText(filePath, "Some text");
                Thread.Sleep(25);

                //Then
                fileSystemEventArgs.Should().NotBeNull();
                fileSystemEventArgs.FullPath.Should().Be(filePath);
            });
        }

        private static IFileSystemWatcher GetFileSystemWatcher()
        {
            return new FileSystemWatcher();
        }
    }
}
