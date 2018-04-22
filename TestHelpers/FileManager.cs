using System.IO;
using System.Runtime.CompilerServices;

namespace TestHelpers
{
    public static class FileManager
    {
        public static string DataFolder = "Data";
        public static string BinarySubFolder = @"bin\Debug";

        public static string GetFullFilePath(string filename, [CallerFilePath] string callerFilePath = null)
        {
            var directory = Path.GetDirectoryName(callerFilePath) ?? "";
            var fullpath = Path.GetFullPath(Path.Combine(directory, BinarySubFolder, DataFolder, filename));
            return fullpath;
        }
    }
}
