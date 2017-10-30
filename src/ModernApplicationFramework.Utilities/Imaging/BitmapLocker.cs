using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ModernApplicationFramework.Utilities.Imaging
{
    public class BitmapLocker : IDisposable
    {
        private readonly Bitmap _bitmap;
        private readonly BitmapData _bitmapData;

        public Bitmap Bitmap => _bitmap;

        public BitmapData BitmapData => _bitmapData;

        public BitmapLocker(Bitmap bitmap) : this (bitmap, ImageLockMode.ReadOnly)
        {
        }

        public BitmapLocker(Bitmap bitmap, ImageLockMode mode)
        {
            Validate.IsNotNull(bitmap, "bitmap");
            _bitmap = bitmap;
            Rectangle rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            _bitmapData = bitmap.LockBits(rect, mode, _bitmap.PixelFormat);
        }

        public BitmapLocker(Bitmap bitmap, ImageLockMode mode, Rectangle rect)
        {
            Validate.IsNotNull(bitmap, "bitmap");
            _bitmap = bitmap;
            _bitmapData = bitmap.LockBits(rect, mode, _bitmap.PixelFormat);
        }

        public void Dispose()
        {
            _bitmap.UnlockBits(_bitmapData);
        }
    }
}
