using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Imaging
{
    public static class ImageConverter
    {
        public static BitmapSource BitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;
            using (var bitmapLocker = new BitmapLocker(bitmap))
            {
                var num = Math.Abs(bitmapLocker.BitmapData.Stride);
                var length = bitmap.Height * num;
                var numArray = new byte[length];
                if (bitmapLocker.BitmapData.Stride > 0)
                {
                    Marshal.Copy(bitmapLocker.BitmapData.Scan0, numArray, 0, length);
                }
                else
                {
                    var scan0 = bitmapLocker.BitmapData.Scan0;
                    for (var index = 0; index < bitmap.Height; ++index)
                    {
                        Marshal.Copy(scan0, numArray, index * num, num);
                        scan0 -= num;
                    }
                }
                var sectionRect = new Int32Rect(0, 0, bitmap.Width, bitmap.Height);
                switch (bitmap.PixelFormat)
                {
                    case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                        numArray = ExtractPixelSection(numArray, 24, num, sectionRect);
                        break;
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                        numArray = ExtractPixelSection(numArray, 32, num, sectionRect);
                        break;
                }
                var bitmapSource = BitmapSource.Create(bitmap.Width, bitmap.Height, DpiHelper.Default.LogicalDpiX, DpiHelper.Default.LogicalDpiY, PixelFormats.Bgra32, null, numArray, num);
                bitmapSource.Freeze();
                return bitmapSource;
            }
        }

        public static Bitmap BitmapFromBitmapSource(BitmapSource bitmapSource)
        {
            if (bitmapSource == null)
                return null;
            System.Drawing.Imaging.PixelFormat format;
            if (bitmapSource.Format == PixelFormats.Bgra32)
                format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            else if (bitmapSource.Format == PixelFormats.Bgr32)
                format = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            else if (bitmapSource.Format == PixelFormats.Pbgra32)
            {
                format = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
            }
            else
            {
                var bgra32 = PixelFormats.Bgra32;
                return BitmapFromBitmapSource(new FormatConvertedBitmap(bitmapSource, bgra32, null, 0.0));
            }
            var pixelWidth = bitmapSource.PixelWidth;
            var pixelHeight = bitmapSource.PixelHeight;
            var stride = pixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
            var bitmap = new Bitmap(pixelWidth, pixelHeight, format);
            using (var bitmapLocker = new BitmapLocker(bitmap, ImageLockMode.ReadWrite))
            {
                var source = new int[pixelWidth * pixelHeight];
                bitmapSource.CopyPixels(source, stride, 0);
                Marshal.Copy(source, 0, bitmapLocker.BitmapData.Scan0, source.Length);
            }
            return bitmap;
        }

        public static byte[] ExtractPixelSection(byte[] sourcePixels, int sourceBitsPerPixel, int sourceStride, Int32Rect sectionRect)
        {
            System.Drawing.Color[] transparentColors = {
                System.Drawing.Color.FromArgb(0, 254, 0),
                System.Drawing.Color.FromArgb(byte.MaxValue, 0, byte.MaxValue)
            };
            return ExtractPixelSection(sourcePixels, sourceBitsPerPixel, sourceStride, sectionRect, transparentColors);
        }

        public static byte[] ExtractPixelSection(byte[] sourcePixels, int sourceBitsPerPixel, int sourceStride, Int32Rect sectionRect, System.Drawing.Color[] transparentColors)
        {
            var length = sectionRect.Width * sectionRect.Height * 4;
            var num1 = sourceBitsPerPixel / 8;
            var flag = false;
            if (sourceBitsPerPixel == 32)
            {
                flag = true;
                var num2 = 0;
                while (num2 < sourcePixels.Length)
                {
                    if (sourcePixels[num2 + 3] != 0)
                    {
                        flag = false;
                        break;
                    }
                    num2 += 4;
                }
            }
            var numArray = new byte[length];
            for (var index1 = 0; index1 < sectionRect.Height; ++index1)
            {
                for (var index2 = 0; index2 < sectionRect.Width; ++index2)
                {
                    var cDisplayClass50 = new ColorClass();
                    var index3 = (index2 + sectionRect.X) * num1 + (index1 + sectionRect.Y) * sourceStride;
                    var index4 = index2 * 4 + index1 * (sectionRect.Width * 4);
                    var num2 = sourcePixels[index3];
                    var num3 = sourcePixels[index3 + 1];
                    var num4 = sourcePixels[index3 + 2];
                    var num5 = sourceBitsPerPixel == 32 ? sourcePixels[index3 + 3] : byte.MaxValue;
                    cDisplayClass50.C = System.Drawing.Color.FromArgb(num4, num3, num2);
                    if (Array.FindIndex(transparentColors, cDisplayClass50.IsEquals) != -1)
                        num5 = 0;
                    else if (flag)
                        num5 = byte.MaxValue;
                    if (num5 == 0)
                    {
                        num2 = 0;
                        num3 = 0;
                        num4 = 0;
                    }
                    numArray[index4] = num2;
                    numArray[index4 + 1] = num3;
                    numArray[index4 + 2] = num4;
                    numArray[index4 + 3] = num5;
                }
            }
            return numArray;
        }

        private sealed class ColorClass
        {
            public System.Drawing.Color C;

            internal bool IsEquals(System.Drawing.Color transparentColor)
            {
                return C == transparentColor;
            }
        }
}
}
