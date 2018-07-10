using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.General
{
    public class BooleanToHiddenVisibilityConverter : ValueConverter<bool, Visibility>
    {
        protected override Visibility Convert(bool value, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Hidden;
            if (value)
                visibility = Visibility.Visible;
            return visibility;
        }
    }
}
