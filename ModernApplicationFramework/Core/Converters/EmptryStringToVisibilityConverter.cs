using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    public class EmptryStringToVisibilityConverter  : ValueConverter<string, Visibility>
    {
        protected override Visibility Convert(string value, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
