using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="IValueConverter"/> that compares the input and an parameter for equality
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IValueConverter" />
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool)value ? parameter : Binding.DoNothing;
        }
    }
}
