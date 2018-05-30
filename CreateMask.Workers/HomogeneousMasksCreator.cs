using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CreateMask.Contracts.Enums;
using CreateMask.Contracts.Helpers;

namespace CreateMask.Workers
{
    public class HomogeneousMasksCreator : IHomogeneousMasksCreator
    {
        public Bitmap CreateMask(Size dimensions, Color color)
        {
            var bitmap = new Bitmap(dimensions.Width, dimensions.Height);
            using (var graphics = Graphics.FromImage(bitmap))
            using(var brush = new SolidBrush(color))
            {
                var point = new Point(0,0);
                var rectangle = new Rectangle(point, dimensions);
                graphics.FillRectangle(brush, rectangle);
            }
            return bitmap;
        }

        public IEnumerable<Bitmap> CreateSetOfMasks(Size dimensions, IntensityDifference intensityDifference)
        {
            var id = (int) intensityDifference;
            var masks = new List<Bitmap>(256/id);
            for (var i = 0; i <= byte.MaxValue; i = i+id)
            {
                var color = Color.FromArgb(i, i, i);
                var mask = CreateMask(dimensions, color);
                masks.Add(mask);
            }
            return masks;
        }

        public void CreateSetInDirectory(string directory, ImageFileType imageFileType, Size dimensions, IntensityDifference intensityDifference)
        {
            var imageFormat = imageFileType.ToImageFormat();

            EnsureDirectoryExists(directory);

            var masks = CreateSetOfMasks(dimensions, intensityDifference);
            foreach (var mask in masks)
            {
                var filename = GetFileName(mask);
                
                mask.Save(filename, imageFormat);
                mask.Dispose();
            }
        }

        private void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private string GetColorString(Color color)
        {
            var colorString = color.R.ToString();
            return colorString;
        }

        private string GetFileName(Bitmap mask)
        {
            var color = mask.GetPixel(0, 0);
            var colorString = GetColorString(color);
            return $"Mask_{mask.Width}x{mask.Height}_{colorString}";
        }
    }
}
