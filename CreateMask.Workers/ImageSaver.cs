using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using CreateMask.Contracts.Interfaces;

namespace CreateMask.Workers
{
    public class ImageSaver : IImageSaver
    {
        public IEnumerable<string> SupportedFileTypes => _supportedFileTypes.Keys;

        private static readonly Dictionary<string, ImageFormat> _supportedFileTypes = new Dictionary<string, ImageFormat>();

        static ImageSaver()
        {
            GetSupportedFileTypes();
        }

        private static void GetSupportedFileTypes()
        {
            var properties = typeof(ImageFormat).GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (var property in properties)
            {
                if (property.PropertyType != typeof(ImageFormat)) continue;
                var imageFormatInstance = (ImageFormat)property.GetValue(null);
                _supportedFileTypes.Add(property.Name.ToLowerInvariant(), imageFormatInstance);
            }
        }

        public void Save(Image image, string filePath, string fileType)
        {
            fileType = fileType.Trim('.', ' ');
            var imageFormat = _supportedFileTypes[fileType];
            image.Save(filePath, imageFormat);
        }
    }
}
