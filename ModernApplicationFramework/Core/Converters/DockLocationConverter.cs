using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters
{
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