using System.Globalization;
using System.Windows;

namespace ModernApplicationFramework.Core.Converters.General
{
    public class VisibleIfNotNullConverter : ValueConverter<object, Visibility>
    {
        protected override Visibility Convert(object value, object parameter, CultureInfo culture)
        {
            var visibility = Visibility.Collapsed;
            if (parameter != null)
                visibility = (Visibility) parameter;
            return value != null ? Visibility.Visible : visibility;
        }
    }
}
