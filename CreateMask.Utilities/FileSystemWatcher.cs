using System.IO;
using CreateMask.Contracts.Interfaces;
using Fsw=System.IO.FileSystemWatcher;

namespace CreateMask.Utilities
{
    public class FileSystemWatcher : IFileSystemWatcher
    {
        private readonly Fsw _fileSystemWachter = new Fsw();

        public event FileSystemEventHandler Created; 

        public void Start(string path)
        {
            if (_fileSystemWachter.EnableRaisingEvents) return;
            _fileSystemWachter.IncludeSubdirectories = false;
            _fileSystemWachter.Path = path;
            _fileSystemWachter.Created += Created;
            _fileSystemWachter.EnableRaisingEvents = true;
        }
    }
}
