using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace ModernApplicationFramework.Utilities.Imaging
{
    public static class ImageThemingUtilities
    {
        public static readonly DependencyProperty ImageBackgroundColorProperty;

        static ImageThemingUtilities()
        {
            ImageBackgroundColorProperty = DependencyProperty.RegisterAttached("ImageBackgroundColor", typeof(Color), typeof(ImageThemingUtilities), new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.Inherits));
            IsImageThemingEnabled  = true;
            conversionBuffer = new ReusableArray<byte>(false);
        }

        public static Color GetImageBackgroundColor(DependencyObject obj)
        {
            var c = (Color)obj.GetValue(ImageBackgroundColorProperty);
            return c;
        }

        public static void SetImageBackgroundColor(DependencyObject obj, Color value)
        {
            obj.SetValue(ImageBackgroundColorProperty, value);
        }

        public static bool IsImageThemingEnabled { get; set; }

        public static Bitmap GetThemedBitmap(Bitmap source, System.Drawing.Color backgroundColor)
        {
            return GetThemedBitmap(source, (uint)ColorTranslator.ToWin32(backgroundColor), SystemParameters.HighContrast);
        }

        public static Bitmap GetThemedBitmap(Bitmap source, System.Drawing.Color backgroundColor, bool isHighContrast)
        {
            return GetThemedBitmap(source, (uint)ColorTranslator.ToWin32(backgroundColor), isHighContrast);
        }

        public static Bitmap GetThemedBitmap(Bitmap source, uint backgroundColor)
        {
            return GetThemedBitmap(source, backgroundColor, SystemParameters.HighContrast);
        }

        public static Bitmap GetThemedBitmap(Bitmap source, uint backgroundColor, bool isHighContrast)
        {
            if (source == null)
                return null;
            Bitmap bitmap = source.Clone() as Bitmap;
            try
            {
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bitmapdata = bitmap.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                int length = Math.Abs(bitmapdata.Stride) * bitmapdata.Height;
                byte[] numArray = new byte[length];
                bool isTopDownBitmap = bitmapdata.Stride > 0;
                Marshal.Copy(bitmapdata.Scan0, numArray, 0, length);
                ThemeDIBits(numArray.Length, numArray, bitmapdata.Width, bitmapdata.Height, isTopDownBitmap, backgroundColor, isHighContrast);
                Marshal.Copy(numArray, 0, bitmapdata.Scan0, length);
                bitmap.UnlockBits(bitmapdata);
            }
            catch
            {
                bitmap?.Dispose();
                bitmap = source.Clone() as Bitmap;
            }
            return bitmap;
        }

        public static bool ThemeDIBits(int pixelCount, byte[] pixels, int width, int height, bool isTopDownBitmap, uint backgroundRgba, bool isHighContrast)
        {
            if (pixelCount != width * height * 4)
                throw new ArgumentException();
            if (!IsImageThemingEnabled || pixels == null)
                return false;
            if (IsOptOutPixelSet(pixels, width, height, isTopDownBitmap))
            {
                ClearOptOutPixel(pixels, width, height, isTopDownBitmap);
                return false;
            }
            HslColor background = HslColor.FromColor(backgroundRgba.ToColorFromRgba());
            int index = 0;
            while (index + 4 <= pixelCount)
            {
                ThemePixel(ref pixels[index + 2], ref pixels[index + 1], ref pixels[index], background, isHighContrast);
                index += 4;
            }
            return true;
        }

        public static unsafe bool IsOptOutPixelSet(byte[] pixels, int width, int height, bool isTopDownBitmap)
        {
            fixed (byte* pPixelBytes = pixels)
                return IsOptOutPixelSet(pPixelBytes, width, height, isTopDownBitmap);
        }

        public static unsafe bool IsOptOutPixelSet(byte* pPixelBytes, int width, int height, bool isTopDownBitmap)
        {
            int offsetToOptOutPixel = ComputeOffsetToOptOutPixel(width, height, isTopDownBitmap);
            return (int)(*(uint*)(pPixelBytes + ((IntPtr)(offsetToOptOutPixel * 4)).ToInt64()) & 16777215U) == ushort.MaxValue;
        }

        private static int ComputeOffsetToOptOutPixel(int width, int height, bool isTopDownBitmap)
        {
            if (isTopDownBitmap)
                return width - 1;
            return width * height - 1;
        }

        public static unsafe void ClearOptOutPixel(byte[] pixels, int width, int height, bool isTopDownBitmap)
        {
            fixed (byte* pPixelBytes = pixels)
                ClearOptOutPixel(pPixelBytes, width, height, isTopDownBitmap);
        }

        public static unsafe void ClearOptOutPixel(byte* pPixelBytes, int width, int height, bool isTopDownBitmap)
        {
            int offsetToOptOutPixel = ComputeOffsetToOptOutPixel(width, height, isTopDownBitmap);
            *(int*)(pPixelBytes + ((IntPtr)(offsetToOptOutPixel * 4)).ToInt64()) = 0;
        }

        public static void ThemePixel(ref byte r, ref byte g, ref byte b, HslColor background)
        {
            ThemePixel(ref r, ref g, ref b, background, SystemParameters.HighContrast);
        }

        public static void ThemePixel(ref byte r, ref byte g, ref byte b, HslColor background, bool isHighContrast)
        {
            HslColor hslColor = HslColor.FromColor(Color.FromRgb(r, g, b));
            double hue1 = hslColor.Hue;
            double saturation1 = hslColor.Saturation;
            double luminosity1 = hslColor.Luminosity;
            double num1 = Math.Abs(82.0 / 85.0 - luminosity1);
            double num2 = Math.Max(0.0, 1.0 - saturation1 * 4.0) * Math.Max(0.0, 1.0 - num1 * 4.0);
            double num3 = Math.Max(0.0, 1.0 - saturation1 * 4.0);
            double luminosity2 = TransformLuminosity(hue1, saturation1, luminosity1, background.Luminosity);
            double hue2 = hue1 * (1.0 - num3) + background.Hue * num3;
            double saturation2 = saturation1 * (1.0 - num2) + background.Saturation * num2;
            if (isHighContrast)
                luminosity2 = (luminosity2 <= 0.3 ? 0.0 : (luminosity2 >= 0.7 ? 1.0 : (luminosity2 - 0.3) / 0.4)) * (1.0 - saturation2) + luminosity2 * saturation2;
            Color color = new HslColor(hue2, saturation2, luminosity2).ToColor();
            r = color.R;
            g = color.G;
            b = color.B;
        }

        private static double TransformLuminosity(double hue, double saturation, double luminosity, double backgroundLuminosity)
        {
            if (backgroundLuminosity < 0.5)
            {
                if (luminosity >= 82.0 / 85.0)
                    return backgroundLuminosity * (luminosity - 1.0) / (-3.0 / 85.0);
                double val2 = saturation >= 0.2 ? (saturation <= 0.3 ? 1.0 - (saturation - 0.2) / (1.0 / 10.0) : 0.0) : 1.0;
                double num1 = Math.Max(Math.Min(1.0, Math.Abs(hue - 37.0) / 20.0), val2);
                double num2 = ((backgroundLuminosity - 1.0) * 0.66 / (82.0 / 85.0) + 1.0) * num1 + 0.66 * (1.0 - num1);
                if (luminosity < 0.66)
                    return (num2 - 1.0) / 0.66 * luminosity + 1.0;
                return (num2 - backgroundLuminosity) / (-259.0 / 850.0) * (luminosity - 82.0 / 85.0) + backgroundLuminosity;
            }
            if (luminosity < 82.0 / 85.0)
                return luminosity * backgroundLuminosity / (82.0 / 85.0);
            return (1.0 - backgroundLuminosity) * (luminosity - 1.0) / (3.0 / 85.0) + 1.0;
        }


        public static BitmapSource CreateThemedBitmapSource(BitmapSource inputImage, Color backgroundColor, bool isEnabled, Color grayscaleBiasColor, bool isHighContrast)
        {
            if (inputImage.Format != PixelFormats.Bgra32)
                inputImage = new FormatConvertedBitmap(inputImage, PixelFormats.Bgra32, null, 0.0);
            int stride = inputImage.PixelWidth * 4;
            int num = inputImage.PixelWidth * inputImage.PixelHeight * 4;
            using (ReusableResourceHolder<byte[]> reusableResourceHolder = AcquireConversionBuffer(num))
            {
                byte[] resource = reusableResourceHolder.Resource;
                inputImage.CopyPixels(resource, stride, 0);
                uint backgroundRgba = (uint)(backgroundColor.B << 16 | backgroundColor.G << 8) | backgroundColor.R;
                ThemeDIBits(num, resource, inputImage.PixelWidth, inputImage.PixelHeight, true, backgroundRgba, isHighContrast);
                if (!isEnabled)
                    GrayscaleDIBits(resource, num, grayscaleBiasColor);
                BitmapSource bitmapSource = BitmapSource.Create(inputImage.PixelWidth, inputImage.PixelHeight, inputImage.DpiX, inputImage.DpiY, PixelFormats.Bgra32, inputImage.Palette, resource, stride);
                bitmapSource.Freeze();
                return bitmapSource;
            }
        }

        private static readonly ReusableArray<byte> conversionBuffer;

        internal static ReusableResourceHolder<byte[]> AcquireConversionBuffer(int size)
        {
            return conversionBuffer.Acquire(size);
        }

        public static void GrayscaleDIBits(byte[] pixels, int pixelLength, Color biasColor)
        {
            Validate.IsNotNull(pixels, "pixels");
            if (pixelLength % 4 != 0)
                throw new ArgumentException("pixels");
            float num1 = biasColor.A / 256f;
            int index = 0;
            while (index + 4 <= pixelLength)
            {
                float num2 = (float)(pixels[index] * 0.000429687497671694 + pixels[index + 1] * 0.00230468739755452 + pixels[index + 2] * 0.00117187504656613);
                pixels[index] = (byte)(num2 * (double)biasColor.B);
                pixels[index + 1] = (byte)(num2 * (double)biasColor.G);
                pixels[index + 2] = (byte)(num2 * (double)biasColor.R);
                pixels[index + 3] = (byte)(num1 * (double)pixels[index + 3]);
                index += 4;
            }
        }
    }
}
