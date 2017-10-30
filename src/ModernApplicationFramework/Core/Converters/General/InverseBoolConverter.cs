using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters.General
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:System.Windows.Data.IValueConverter" /> that inverses a <see langword="bool" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IValueConverter" />
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}