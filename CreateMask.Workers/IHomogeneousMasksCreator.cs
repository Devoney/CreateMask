using System.Collections.Generic;
using System.Drawing;
using CreateMask.Contracts.Enums;

namespace CreateMask.Workers
{
    public interface IHomogeneousMasksCreator
    {
        Bitmap CreateMask(Size dimensions, Color color);
        IEnumerable<Bitmap> CreateSetOfMasks(Size dimensions, IntensityDifference intensityDifference);
        void CreateSetInDirectory(string directory, ImageFileType imageFileType, Size dimensions, IntensityDifference intensityDifference);
    }
}