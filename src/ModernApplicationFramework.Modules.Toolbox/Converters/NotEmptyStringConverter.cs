using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernApplicationFramework.Modules.Toolbox.Converters
{
    public class NotEmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                throw new ArgumentException("Value to convert must be a string", nameof(value));
            return !string.IsNullOrWhiteSpace((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
