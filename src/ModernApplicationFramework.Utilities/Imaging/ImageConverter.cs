using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Utilities.Imaging
{
    public static class ImageConverter
    {
        public static BitmapSource BitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;
            using (var bitmapLocker = new BitmapLocker(bitmap))
            {
                int num = Math.Abs(bitmapLocker.BitmapData.Stride);
                int length = bitmap.Height * num;
                byte[] numArray = new byte[length];
                if (bitmapLocker.BitmapData.Stride > 0)
                {
                    Marshal.Copy(bitmapLocker.BitmapData.Scan0, numArray, 0, length);
                }
                else
                {
                    IntPtr scan0 = bitmapLocker.BitmapData.Scan0;
                    for (int index = 0; index < bitmap.Height; ++index)
                    {
                        Marshal.Copy(scan0, numArray, index * num, num);
                        scan0 -= num;
                    }
                }
                Int32Rect sectionRect = new Int32Rect(0, 0, bitmap.Width, bitmap.Height);
                switch (bitmap.PixelFormat)
                {
                    case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                        numArray = ExtractPixelSection(numArray, 24, num, sectionRect);
                        break;
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                        numArray = ExtractPixelSection(numArray, 32, num, sectionRect);
                        break;
                }
                BitmapSource bitmapSource = BitmapSource.Create(bitmap.Width, bitmap.Height, DpiHelper.Default.LogicalDpiX, DpiHelper.Default.LogicalDpiY, PixelFormats.Bgra32, null, numArray, num);
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
                System.Windows.Media.PixelFormat bgra32 = PixelFormats.Bgra32;
                return BitmapFromBitmapSource(new FormatConvertedBitmap(bitmapSource, bgra32, null, 0.0));
            }
            int pixelWidth = bitmapSource.PixelWidth;
            int pixelHeight = bitmapSource.PixelHeight;
            int stride = pixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
            Bitmap bitmap = new Bitmap(pixelWidth, pixelHeight, format);
            using (BitmapLocker bitmapLocker = new BitmapLocker(bitmap, ImageLockMode.ReadWrite))
            {
                int[] source = new int[pixelWidth * pixelHeight];
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
            int length = sectionRect.Width * sectionRect.Height * 4;
            int num1 = sourceBitsPerPixel / 8;
            bool flag = false;
            if (sourceBitsPerPixel == 32)
            {
                flag = true;
                int num2 = 0;
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
            byte[] numArray = new byte[length];
            for (int index1 = 0; index1 < sectionRect.Height; ++index1)
            {
                for (int index2 = 0; index2 < sectionRect.Width; ++index2)
                {
                    var cDisplayClass50 = new ColorClass();
                    int index3 = (index2 + sectionRect.X) * num1 + (index1 + sectionRect.Y) * sourceStride;
                    int index4 = index2 * 4 + index1 * (sectionRect.Width * 4);
                    byte num2 = sourcePixels[index3];
                    byte num3 = sourcePixels[index3 + 1];
                    byte num4 = sourcePixels[index3 + 2];
                    byte num5 = sourceBitsPerPixel == 32 ? sourcePixels[index3 + 3] : byte.MaxValue;
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
