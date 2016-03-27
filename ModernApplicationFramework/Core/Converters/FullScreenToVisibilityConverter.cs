using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters
{
    class FullScreenToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool)
                flag = (bool)value;
            return (Visibility)(flag ? 2 : 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
