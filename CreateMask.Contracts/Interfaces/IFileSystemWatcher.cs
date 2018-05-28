using System.IO;

namespace CreateMask.Contracts.Interfaces
{
    public interface IFileSystemWatcher
    {
        event FileSystemEventHandler Created;
        void Start(string directory);
    }
}
