using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Imaging
{
    public class GrayscaleBitmapSourceConverter : ValueConverter<BitmapSource, BitmapSource>
    {
        private static readonly Color DefaultBiasColor = Colors.White;

        protected override BitmapSource Convert(BitmapSource image, object parameter, CultureInfo culture)
        {
            return ConvertCore(image, GetBiasColor(parameter));
        }

        public static BitmapSource ConvertCore(BitmapSource image, Color biasColor)
        {
            if (image == null)
                return null;
            BitmapSource bitmapSource;
            if (image.Format == PixelFormats.Bgra32 && image.PixelWidth <= 128 && image.PixelHeight <= 128)
            {
                bitmapSource = ConvertToGrayScale(image, biasColor);
            }
            else
            {
                if (biasColor.R != byte.MaxValue || biasColor.G != byte.MaxValue || biasColor.B != byte.MaxValue)
                    throw new NotSupportedException();
                FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap();
                formatConvertedBitmap.BeginInit();
                formatConvertedBitmap.DestinationFormat = PixelFormats.Gray32Float;
                formatConvertedBitmap.Source = image;
                formatConvertedBitmap.EndInit();
                if (formatConvertedBitmap.CanFreeze)
                    formatConvertedBitmap.Freeze();
                bitmapSource = formatConvertedBitmap;
            }
            return bitmapSource;
        }

        public static Color GetBiasColor(object parameter)
        {
            if (parameter is Color color)
                return color;
            return DefaultBiasColor;
        }

        private static unsafe BitmapSource ConvertToGrayScale(BitmapSource image, Color biasColor)
        {
            Validate.IsNotNull(image, nameof(image));
            if (image.Format != PixelFormats.Bgra32)
                throw new ArgumentException();
            int stride = image.PixelWidth * 4;
            int num = image.PixelWidth * image.PixelHeight * 4;
            using (ReusableResourceHolder<byte[]> reusableResourceHolder = ImageThemingUtilities.AcquireConversionBuffer(num))
            {
                byte[] resource = reusableResourceHolder.Resource;
                image.CopyPixels(resource, stride, 0);
                ImageThemingUtilities.GrayscaleDIBits(resource, num, biasColor);
                byte[] numArray;
                BitmapSource bitmapSource;
                fixed (void* _ = numArray = resource)
                {
                    void* voidPtr;
                    if (numArray.Length == 0)
                        voidPtr = null;
                    else
                        fixed (void* tmp = &numArray[0])
                        {
                            voidPtr = tmp;
                        }
                    bitmapSource = BitmapSource.Create(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, PixelFormats.Bgra32, image.Palette, (IntPtr)voidPtr, num, stride);
                }

                bitmapSource.Freeze();
                return bitmapSource;
            }
        }
    }
}
