using System.IO;
using Fsw=System.IO.FileSystemWatcher;

namespace CreateMask.Utilities
{
    public class FileSystemWatcher
    {
        private readonly Fsw _fileSystemWachter = new Fsw();

        public event FileSystemEventHandler Created; 

        public void Start(string path, bool includeSubdirectories = false)
        {
            if (_fileSystemWachter.EnableRaisingEvents) return;
            _fileSystemWachter.IncludeSubdirectories = includeSubdirectories;
            _fileSystemWachter.Path = path;
            _fileSystemWachter.Created += Created;
            _fileSystemWachter.EnableRaisingEvents = true;
        }
    }
}
