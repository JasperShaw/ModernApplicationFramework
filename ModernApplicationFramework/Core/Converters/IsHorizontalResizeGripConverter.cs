using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters
{
    internal class IsHorizontalResizeGripConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int) (double) value >= 10)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}