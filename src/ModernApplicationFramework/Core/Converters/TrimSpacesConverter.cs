using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    internal class TrimSpacesConverter : ValueConverter<string, string>
    {
        protected override string Convert(string value, object parameter, CultureInfo culture)
        {
            return value.Trim();
        }

        protected override string ConvertBack(string value, object parameter, CultureInfo culture)
        {
            return value.Trim();
        }
    }
}