using System.Globalization;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Imaging.Converters;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Imaging
{
    public class DpiPrescaleThemedImageSourceConverter : MultiValueConverter<ImageSource, Color, bool, ImageSource>
    {
        protected virtual DpiHelper DpiHelper => DpiHelper.Default;

        protected override ImageSource Convert(ImageSource value1, Color value2, bool value3, object parameter, CultureInfo culture)
        {
            return InternalConvert(DpiHelper, value1, value2, value3, parameter, culture);
        }

        internal static ImageSource InternalConvert(DpiHelper dpiHelper, ImageSource inputImage, Color backgroundColor, bool isEnabled, object parameter, CultureInfo culture)
        {
            return InternalConvert(dpiHelper, inputImage, backgroundColor, isEnabled, SystemParameters.HighContrast, parameter, culture);
        }

        internal static ImageSource InternalConvert(DpiHelper dpiHelper, ImageSource inputImage, Color backgroundColor, bool isEnabled, bool isHighContrast, object parameter, CultureInfo culture)
        {
            ImageSource inputImage1 = ThemedImageSourceConverter.ConvertCore(inputImage, backgroundColor, isEnabled, isHighContrast, parameter);
            return DpiPrescaleImageSourceConverter.InternalConvert(dpiHelper, inputImage1);
        }
    }
}
