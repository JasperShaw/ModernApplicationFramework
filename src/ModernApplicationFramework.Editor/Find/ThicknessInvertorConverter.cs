using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernApplicationFramework.Editor.Find
{
    internal class ThicknessInvertorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness thickness = (Thickness)value;
            if (thickness.Left != 0.0 || thickness.Top != 0.0 || (thickness.Right != 0.0 || thickness.Bottom != 0.0))
                return new Thickness(0.0);
            return new Thickness(1.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
