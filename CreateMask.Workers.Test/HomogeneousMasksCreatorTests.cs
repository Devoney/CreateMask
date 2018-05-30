using System.Drawing;
using System.Linq;
using CreateMask.Contracts.Enums;
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
            AssertExt.Equals(expectedBitmap, bitmap);
        }

        [Test, Category(Categories.Unit)]
        [TestCase(IntensityDifference.Id1, 256)]
        [TestCase(IntensityDifference.Id8, 32)]
        [TestCase(IntensityDifference.Id64, 4)]
        public void CorrectNumberOfMasksIsCreated(IntensityDifference intensityDifference, int expectedNrOfMasks)
        {
            //Given
            var hmc = GetHomogeneousMasksCreator();
            var size = new Size(1, 1);

            //When
            var masks = hmc.CreateSetOfMasks(size, intensityDifference);

            //Then
            masks.Count().Equals(expectedNrOfMasks);
        }

        private IHomogeneousMasksCreator GetHomogeneousMasksCreator()
        {
            return new HomogeneousMasksCreator();
        }
    }
}
