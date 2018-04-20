using System;
using System.Drawing;
using NUnit.Framework;

namespace TestHelpers
{
    public static class AssertExt
    {
        public static void ThrowsException<T>(Action action, string containsPartialMessage) where T:Exception
        {
            var exception = default(T);
            try
            {
                action();
            }
            catch (T ex)
            {
                exception = ex;
            }
            Assert.IsNotNull(exception, $"An exception of type '{typeof(T).FullName}' is expected but was not thrown.");
            Assert.IsTrue(exception.Message.Contains(containsPartialMessage), $"Exception message '{exception.Message}' does not contain partial message '{containsPartialMessage}'.");
        }

        public static void Equals(this Bitmap expectedBitmap, Bitmap actualBitmap)
        {
            for (var x = 0; x < expectedBitmap.Width; x++)
            {
                for (var y = 0; y < expectedBitmap.Height; y++)
                {
                    var expectedColor = expectedBitmap.GetPixel(x, y);
                    var actualColor = actualBitmap.GetPixel(x, y);
                    Assert.AreEqual(expectedColor, actualColor);
                }
            }
        }
    }
}
