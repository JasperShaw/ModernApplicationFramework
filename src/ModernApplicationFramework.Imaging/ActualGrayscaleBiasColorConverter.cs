using System.Globalization;
using System.Windows.Media;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Imaging
{
    public sealed class ActualGrayscaleBiasColorConverter : MultiValueConverter<Color?, bool, Color>
    {
        public static readonly ActualGrayscaleBiasColorConverter Instance = new ActualGrayscaleBiasColorConverter();

        protected override Color Convert(Color? grayscaleBiasColor, bool highContrast, object parameter, CultureInfo culture)
        {
            if (grayscaleBiasColor.HasValue)
                return grayscaleBiasColor.Value;
            if (highContrast)
                return ImageLibrary.HighContrastGrayscaleBiasColor;
            return ImageLibrary.DefaultGrayscaleBiasColor;
        }
    }
}
