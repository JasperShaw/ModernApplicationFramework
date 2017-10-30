using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:System.Windows.Data.IValueConverter" /> that casts the input to <see cref="T:System.Windows.Controls.Dock" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IValueConverter" />
    internal class DockLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Dock)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Dock)value;
        }
    }
}