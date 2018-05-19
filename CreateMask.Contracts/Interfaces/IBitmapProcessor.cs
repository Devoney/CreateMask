using System.Drawing;
using System.Drawing.Imaging;

namespace CreateMask.Contracts.Interfaces
{
    public interface IBitmapProcessor
    {
        Bitmap Resize(Bitmap bitmap, int width, int height);
        void Save(Bitmap bitmap, string filePath, ImageFormat imageFormat);
    }
}
