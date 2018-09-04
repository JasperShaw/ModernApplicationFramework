using System.Globalization;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Imaging
{
    public class DpiPrescaleImageSourceConverter : ValueConverter<ImageSource, ImageSource>
    {
        protected virtual DpiHelper DpiHelper => DpiHelper.Default;

        protected override ImageSource Convert(ImageSource inputImage, object parameter, CultureInfo culture)
        {
            return InternalConvert(DpiHelper, inputImage);
        }

        internal static ImageSource InternalConvert(DpiHelper dpiHelper, ImageSource inputImage)
        {
            if (inputImage == null)
                return null;
            if (!dpiHelper.UsePreScaledImages)
                return inputImage;
            int num = dpiHelper.DpiScalePercentX / 100;
            if (num <= 1)
                return inputImage;
            return dpiHelper.ScaleLogicalImageForDeviceSize(inputImage, new Size(inputImage.Width * num, inputImage.Height * num), BitmapScalingMode.NearestNeighbor);
        }
    }
}
