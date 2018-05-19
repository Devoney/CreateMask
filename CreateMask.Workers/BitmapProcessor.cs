using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using CreateMask.Contracts.Interfaces;

namespace CreateMask.Workers
{
    public class BitmapProcessor : IBitmapProcessor
    {
        public Bitmap Resize(Bitmap bitmap, int width, int height)
        {
            var destination = new Bitmap(width, height);
            using (var gr = Graphics.FromImage(destination))
            using (var imageAttributes = new ImageAttributes())
            {
                imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBilinear;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(bitmap, new Rectangle(0, 0, width, height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            return destination;
        }

        public void Save(Bitmap bitmap, string filePath, ImageFormat imageFormat)
        {
            bitmap.Save(filePath, imageFormat);
        }
    }
}
