using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Workers.Test
{
    [TestFixture]
    public class ImageSaverTests
    {
        [Test]
        public void SupportedFileTypesAreReturnedCorrectly()
        {
            //Given
            var imageSaver = GetImageSaver();
            var expectedSUpportedFileTypes = GetSupportedFileTypes().ToList();
            //Just check some extensions to be sure.
            expectedSUpportedFileTypes.Should().Contain(new[]
            {
                nameof(ImageFormat.Bmp).ToLowerInvariant(),
                nameof(ImageFormat.Exif).ToLowerInvariant(),
                nameof(ImageFormat.Gif).ToLowerInvariant(),
                nameof(ImageFormat.Icon).ToLowerInvariant(),
                nameof(ImageFormat.Jpeg).ToLowerInvariant(),
                nameof(ImageFormat.Png).ToLowerInvariant(),
                nameof(ImageFormat.Tiff).ToLowerInvariant()
            });
            
            //When
            var actualFileTypes = imageSaver.SupportedFileTypes;

            //Then
            actualFileTypes.Should().BeEquivalentTo(expectedSUpportedFileTypes);
        }

        [Test]
        public void ImageCanBeSaved()
        {
            //Given
            var savePath = FileManager.GetFullFilePath("TestSavePathImage.bmp");
            File.Delete(savePath);
            var imagePath = FileManager.GetFullFilePath("ImageSaverTestFile.bmp");
            var expectedImage = Image.FromFile(imagePath);
            expectedImage.Should().NotBeNull();
            var imageSaver = GetImageSaver();
            const string fileType = " .bmp ";

            //When
            imageSaver.Save(expectedImage, savePath, fileType);

            //Then
            var actualImage = Image.FromFile(savePath);
            AssertExt.Equals(expectedImage, actualImage);
        }

        private static IImageSaver GetImageSaver()
        {
            return new ImageSaver();
        }

        private static IEnumerable<string> GetSupportedFileTypes()
        {
            var properties = typeof(ImageFormat).GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (var property in properties)
            {
                if (property.PropertyType != typeof(ImageFormat)) continue;
                yield return property.Name.ToLowerInvariant();
            }
        }
    }
}
