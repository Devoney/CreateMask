using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace TestHelpers
{
    public static class StorageManager
    {
        public static string DataFolder = "Data";
        public static string BinarySubFolder = @"bin\Debug";

        public static string GetFullFilePath(string filename, [CallerFilePath] string callerFilePath = null)
        {
            var directory = Path.GetDirectoryName(callerFilePath) ?? "";
            var fullpath = Path.GetFullPath(Path.Combine(directory, BinarySubFolder, DataFolder, filename));
            return fullpath;
        }

        public static void InTemporaryDirectory(Action<string> action)
        {
            var dirName = Guid.NewGuid().ToString().Substring(0, 8);
            var directory = Path.Combine(Path.GetTempPath(), dirName);
            Directory.CreateDirectory(directory);

            try
            {
                action(directory);
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }
    }
}
