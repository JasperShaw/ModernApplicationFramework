using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.General
{
    public class IsEqualConverter : ValueConverter<object, bool>
    {
        protected override bool Convert(object value, object parameter, CultureInfo culture)
        {
            if (Equals(value, parameter))
                return true;
            if (value != null)
                return value.Equals(parameter);
            return false;
        }

        protected override object ConvertBack(bool value, object parameter, CultureInfo culture)
        {
            if (value)
                return parameter;
            return DependencyProperty.UnsetValue;
        }
    }
}
