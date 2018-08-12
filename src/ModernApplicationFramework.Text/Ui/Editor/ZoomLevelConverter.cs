using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [ValueConversion(typeof(double), typeof(string))]
    public sealed class ZoomLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            if (value != null)
                return string.Format(culture, "{0:F0} " + culture.NumberFormat.PercentSymbol, (double)value);
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            if (value != null && double.TryParse(value.ToString().Replace(culture.NumberFormat.PercentSymbol, string.Empty).Trim(), out var result))
                return result;
            return DependencyProperty.UnsetValue;
        }
    }
}
