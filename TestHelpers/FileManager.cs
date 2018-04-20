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
            if (!File.Exists(fullpath))
            {
                throw new FileNotFoundException($"Unit test file '{callerFilePath}' attempts to load file '{filename}', but wasn't found in dir '{directory}'.");
            }
            return fullpath;
        }
    }
}
