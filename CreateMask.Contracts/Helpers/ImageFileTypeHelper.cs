using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using CreateMask.Contracts.Enums;

namespace CreateMask.Contracts.Helpers
{
    public static class ImageFileTypeHelper
    {
        static ImageFileTypeHelper()
        {
            var imageFileTypes = new List<string>();
            foreach (ImageFileType imageFileType in Enum.GetValues(typeof (ImageFileType)))
            {
                imageFileTypes.Add(imageFileType.ToString());
            }
            ImageFileTypes = imageFileTypes.ToArray();
        }

        public static string[] ImageFileTypes { get; private set; }

        public static ImageFormat ToImageFormat(this ImageFileType imageFileType)
        {
            switch (imageFileType)
            {
                case ImageFileType.Bmp:
                    return ImageFormat.Bmp;
                case ImageFileType.Gif:
                    return ImageFormat.Gif;
                case ImageFileType.Jpeg:
                    return ImageFormat.Jpeg;
                case ImageFileType.Png:
                    return ImageFormat.Png;
                case ImageFileType.Tiff:
                    return ImageFormat.Tiff;
                default:
                    throw new ArgumentOutOfRangeException(nameof(imageFileType), imageFileType, null);
            }
        }

        public static ImageFileType FromString(string fileType)
        {
            var filtered = fileType.Replace(".", "").Replace(" ", "").ToLowerInvariant();
            ImageFileType imageFileType;
            if (!Enum.TryParse(filtered, true, out imageFileType))
            {
                throw new InvalidOperationException($"Could not parse string value '{fileType}' to a value of '{typeof(ImageFileType).FullName}'");
            }
            return imageFileType;
        }
    }
}