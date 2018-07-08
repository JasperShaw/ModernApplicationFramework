using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Utilities.Imaging;

namespace ModernApplicationFramework.Utilities.Converters
{
    public class ThemedImageSourceConverter : MultiValueConverter<ImageSource, Color, bool, ImageSource>
    {
        public static ImageSource ConvertCore(ImageSource inputImage, Color backgroundColor, bool isEnabled, bool isHighContrast, object parameter)
        {
            if (!(inputImage is BitmapSource inputImage1) || backgroundColor.A == 0 & isEnabled)
                return inputImage;
            var biasColor = GrayscaleBitmapSourceConverter.GetBiasColor(parameter);
            return ImageThemingUtilities.GetOrCreateThemedBitmapSource(inputImage1, backgroundColor, isEnabled, biasColor, isHighContrast);
        }

        public static ImageSource ConvertCore(ImageSource inputImage, Color backgroundColor, bool isEnabled, object parameter)
        {
            return ConvertCore(inputImage, backgroundColor, isEnabled, SystemParameters.HighContrast, parameter);
        }

        protected override ImageSource Convert(ImageSource inputImage, Color backgroundColor, bool isEnabled, object parameter, CultureInfo culture)
        {
            return ConvertCore(inputImage, backgroundColor, isEnabled, parameter);
        }
    }
}
