using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CreateMask.Contracts.Enums;
using CreateMask.Contracts.Helpers;
using CreateMask.Contracts.Interfaces;

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
            var nrOfMasks = (256/id) + 1;
            if (intensityDifference == IntensityDifference.Id1) nrOfMasks = 256;
            var masks = new List<Bitmap>(nrOfMasks);
            var pixelColorValue = -id;
            for (var i = 0; i < nrOfMasks; i++)
            {
                pixelColorValue = pixelColorValue + id;
                var color = Color.FromArgb(pixelColorValue, pixelColorValue, pixelColorValue);
                var mask = CreateMask(dimensions, color);
                masks.Add(mask);
                if (i == 0)
                {
                    pixelColorValue--;
                }
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
                var filename = GetFileName(mask, imageFileType);
                var filePath = Path.Combine(directory, filename);
                
                mask.Save(filePath, imageFormat);
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
            while (colorString.Length < 3)
            {
                colorString = "0" + colorString;
            }
            return colorString;
        }

        private string GetFileName(Bitmap mask, ImageFileType imageFileType)
        {
            var color = mask.GetPixel(0, 0);
            var colorString = GetColorString(color);
            return $"Mask_{mask.Width}x{mask.Height}_{colorString}.{imageFileType.ToString().ToLowerInvariant()}";
        }
    }
}
