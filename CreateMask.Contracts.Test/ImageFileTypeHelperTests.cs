using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using CreateMask.Contracts.Enums;
using CreateMask.Contracts.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Contracts.Test
{
    [TestFixture]
    public class ImageFileTypeHelperTests
    {
        [Test]
        [TestCase(ImageFileType.Png, "png")]
        [TestCase(ImageFileType.Bmp, "BmP")]
        [TestCase(ImageFileType.Tiff, ".Tiff")]
        [TestCase(ImageFileType.Jpeg, " .jpeg ")]
        public void ReturnsCorrectImageFileTypeFromString(ImageFileType expectedImageFileType, string stringValue)
        {
            //When
            var actualImageFileType = ImageFileTypeHelper.FromString(stringValue);

            //Then
            actualImageFileType.Should().BeEquivalentTo(expectedImageFileType);
        }

        [Test]
        public void ThrowsExceptionWhenStringCouldNotBeParsedToImageFileType()
        {
            //Given
            var invalidImageFileTypeString = "test";
            var expectedExceptionMessage = $"Could not parse string value '{invalidImageFileTypeString}' to a value of '{typeof (ImageFileType).FullName}'";

            //When
            var action = new Action(() =>
            {
                ImageFileTypeHelper.FromString(invalidImageFileTypeString);
            });

            //Then
            AssertExt.ThrowsException<InvalidOperationException>(action, expectedExceptionMessage);
        }

        [Test]
        public void ImageFileTypeCanBeConvertedToImageFormat()
        {
            //Given
            var dict = new Dictionary<ImageFileType, ImageFormat>
            {
                { ImageFileType.Bmp, ImageFormat.Bmp },
                { ImageFileType.Gif, ImageFormat.Gif },
                { ImageFileType.Jpeg, ImageFormat.Jpeg },
                { ImageFileType.Png, ImageFormat.Png },
                { ImageFileType.Tiff, ImageFormat.Tiff },
            };
            var error = false;

            //When
            foreach (var kvp in dict)
            {
                try
                {
                    var imageFormat = kvp.Key.ToImageFormat();
                    if (!imageFormat.Equals(kvp.Value))
                    {
                        Console.WriteLine($"ImageFileType '{kvp.Key}' did not convert to the correct '{typeof(ImageFormat).FullName}'.");
                        error = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    error = true;
                }
            }

            //Then
            Assert.IsFalse(error, "See trace output.");
        }
    }
}
