using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CreateMask.Utilities
{
    public static class Image
    {
        public static Bitmap Resize(this Bitmap source, Size size)
        {
            return Resize(source, size.Width, size.Height);
        }

        public static Bitmap Resize(this Bitmap source, int width, int height)
        {
            var destination = new Bitmap(width, height);
            using (var gr = Graphics.FromImage(destination))
            using (var imageAttributes = new ImageAttributes())
            {
                imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBilinear;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(source, new Rectangle(0, 0, width, height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            return destination;
        }
    }
}
