using System;
using System.Drawing;
using System.Drawing.Imaging;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Imaging
{
    public class BitmapLocker : IDisposable
    {
        public Bitmap Bitmap { get; }

        public BitmapData BitmapData { get; }

        public BitmapLocker(Bitmap bitmap) : this (bitmap, ImageLockMode.ReadOnly)
        {
        }

        public BitmapLocker(Bitmap bitmap, ImageLockMode mode)
        {
            Validate.IsNotNull(bitmap, "bitmap");
            Bitmap = bitmap;
            Rectangle rect = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
            BitmapData = bitmap.LockBits(rect, mode, Bitmap.PixelFormat);
        }

        public BitmapLocker(Bitmap bitmap, ImageLockMode mode, Rectangle rect)
        {
            Validate.IsNotNull(bitmap, "bitmap");
            Bitmap = bitmap;
            BitmapData = bitmap.LockBits(rect, mode, Bitmap.PixelFormat);
        }

        public void Dispose()
        {
            Bitmap.UnlockBits(BitmapData);
        }
    }
}
