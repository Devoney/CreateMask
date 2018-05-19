using System;
using System.Collections.Generic;
using System.IO;

namespace TestHelpers
{
    public class TempFileManager : IDisposable //Kind of abusing disposable, but hey, be creative.
    {
        private readonly string _directory;
        private readonly List<string> _files = new List<string>();
        
        public TempFileManager(string directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException("The directory specifiedd should exist already.");
            }
            _directory = directory;
        }

        public string GetTempFile()
        {
            var fileName = Guid.NewGuid().ToString().Substring(0, 10);
            var fullPath = Path.Combine(_directory, fileName);
            File.Create(fullPath).Dispose();
            _files.Add(fullPath);
            return fullPath;
        }

        public string GetTempFile(string fileContents)
        {
            var tempFile = GetTempFile();
            File.WriteAllText(tempFile, fileContents);
            return tempFile;
        }

        public void Dispose()
        {
            foreach (var file in _files)
            {
                File.Delete(file);
            }
        }
    }
}
