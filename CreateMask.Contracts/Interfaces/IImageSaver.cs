using System.Collections.Generic;
using System.Drawing;

namespace CreateMask.Contracts.Interfaces
{
    public interface IImageSaver
    {
        IEnumerable<string> SupportedFileTypes { get; } 
        void Save(Image image, string filePath, string fileType);
    }
}
