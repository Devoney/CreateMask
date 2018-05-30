using System.Drawing;
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
            using (Bitmap expectedBitmap = (Bitmap)Image.FromFile(filePath))
            {

                //When
                using (var bitmap = hmc.CreateMask(size, color))
                {
                    //Then
                    AssertExt.Equals(expectedBitmap, bitmap);
                }
            }
        }

        private IHomogeneousMasksCreator GetHomogeneousMasksCreator()
        {
            return new HomogeneousMasksCreator();
        }
    }
}
