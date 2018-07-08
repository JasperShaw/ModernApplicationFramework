using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Imaging.Converters
{
    public sealed class ActualHighContrastConverter : MultiValueConverter<bool?, bool, bool>
    {
        public static readonly ActualHighContrastConverter Instance = new ActualHighContrastConverter();

        protected override bool Convert(bool? highContrast, bool systemHighContrast, object parameter, CultureInfo culture)
        {
            if (highContrast.HasValue)
                return highContrast.Value;
            return systemHighContrast;
        }
    }
}
