using System;
using System.Drawing;
using System.IO;
using System.Linq;
using CreateMask.Contracts.Enums;
using CreateMask.Contracts.Interfaces;
using FluentAssertions;
using NUnit.Framework;
using TestHelpers;

namespace CreateMask.Workers.Test
{
    [TestFixture]
    public class HomogeneousMasksCreatorTests
    {
        [Test, Category(Categories.Unit)]
        public void HomogeneousMaskIsCreatedCorrectly()
        {
            //Given
            var hmc = GetHomogeneousMasksCreator();
            var size = new Size(32, 48);
            var color = Color.FromArgb(56, 56, 56);
            var filePath = StorageManager.GetFullFilePath("homogeneousmask32x48-56.bmp");
            var expectedBitmap = Image.FromFile(filePath);

            //When
            var bitmap = hmc.CreateMask(size, color);

            //Then
            Equals(expectedBitmap, bitmap);
        }

        [Test, Category(Categories.Unit)]
        [TestCase(IntensityDifference.Id1, 256)]
        [TestCase(IntensityDifference.Id2, 129)]
        [TestCase(IntensityDifference.Id8, 33)]
        [TestCase(IntensityDifference.Id64, 5)]
        public void CorrectNumberOfMasksIsCreated(IntensityDifference intensityDifference, int expectedNrOfMasks)
        {
            //Given
            var hmc = GetHomogeneousMasksCreator();
            var size = new Size(1, 1);

            //When
            var masks = hmc.CreateSetOfMasks(size, intensityDifference);

            //Then
            masks.Count().Should().Be(expectedNrOfMasks);
        }

        [Test, Category(Categories.Unit)]
        public void DirectoryIsCreatedIfItDoesNotExistAlready()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                var hmc = GetHomogeneousMasksCreator();
                var directoryPath = Path.Combine(directory, "dir");
                var size = new Size(1, 1);

                //When
                hmc.CreateSetInDirectory(directoryPath, ImageFileType.Bmp, size, IntensityDifference.Id64);

                //Then
                Directory.Exists(directoryPath).Should().BeTrue();
            });
        }

        [Test, Category(Categories.Unit)]
        public void CorrectColorsAreUsedInSetOfMasksInDirectory()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                var hmc = GetHomogeneousMasksCreator();
                var directoryPath = Path.Combine(directory, "dir");
                var size = new Size(1, 1);
                var expectedColors = new[]
                {
                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(63, 63, 63),
                    Color.FromArgb(127, 127, 127),
                    Color.FromArgb(191, 191, 191),
                    Color.FromArgb(255, 255, 255),
                };

                //When
                hmc.CreateSetInDirectory(directoryPath, ImageFileType.Bmp, size, IntensityDifference.Id64);

                //Then
                var files = Directory.GetFiles(directoryPath).ToList();
                files.Count.Should().Be(expectedColors.Length);
                files.Sort();
                for (var i = 0; i < expectedColors.Length; i++)
                {
                    var expectedColor = expectedColors[i];
                    using (var bitmap = (Bitmap)Image.FromFile(files[i]))
                    {
                        var pixelColor = bitmap.GetPixel(0, 0);
                        pixelColor.Should().BeEquivalentTo(expectedColor);
                    }
                }
            });
        }

        [Test, Category(Categories.Unit)]
        public void CorrectNamesAreUsedForFilesInDirectory()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                var hmc = GetHomogeneousMasksCreator();
                var directoryPath = Path.Combine(directory, "dir");
                var size = new Size(1, 2);
                var expectedNames = new[]
                {
                    "Mask_1x2_000.bmp",
                    "Mask_1x2_063.bmp",
                    "Mask_1x2_127.bmp",
                    "Mask_1x2_191.bmp",
                    "Mask_1x2_255.bmp",
                };

                //When
                hmc.CreateSetInDirectory(directoryPath, ImageFileType.Bmp, size, IntensityDifference.Id64);

                //Then
                var files = Directory.GetFiles(directoryPath).ToList();
                files.Count.Should().Be(expectedNames.Length);
                files.Sort();
                for (var i = 0; i < expectedNames.Length; i++)
                {
                    var expectedName = expectedNames[i];
                    var actualName = Path.GetFileName(files[i]);
                    actualName.Should().Be(expectedName);
                }
            });
        }

        [Test, Category(Categories.Unit)]
        public void FilesAreOverwrittenInDirectory()
        {
            StorageManager.InTemporaryDirectory(directory =>
            {
                //Given
                var hmc = GetHomogeneousMasksCreator();
                var names = new[]
                {
                    "Mask_1x1_000.bmp",
                    "Mask_1x1_063.bmp",
                    "Mask_1x1_127.bmp",
                    "Mask_1x1_191.bmp",
                    "Mask_1x1_255.bmp",
                };
                foreach (var name in names)
                {
                    var filepath = Path.Combine(directory, name);
                    File.WriteAllText(filepath, "This should be overwritten.");
                    IsValidImage(filepath).Should().BeFalse();
                }

                //When
                hmc.CreateSetInDirectory(directory, ImageFileType.Bmp, new Size(1, 1), IntensityDifference.Id64);

                //Then 
                var files = Directory.GetFiles(directory);
                files.All(IsValidImage).Should().BeTrue();
            });
        }

        #region Helpers
        private static bool IsValidImage(string file)
        {
            try
            {
                using (var image = Image.FromFile(file))
                {
                    image.Should().NotBeNull();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static IHomogeneousMasksCreator GetHomogeneousMasksCreator()
        {
            return new HomogeneousMasksCreator();
        }
        #endregion
    }
}
