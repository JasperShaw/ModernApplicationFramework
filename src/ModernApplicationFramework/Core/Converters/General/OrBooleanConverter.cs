using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.Converters.General
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:System.Windows.Data.IMultiValueConverter" /> that simulates as a logical OR gate
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IMultiValueConverter" />
    public sealed class OrBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;
            foreach (var obj in values)
            {
                if (obj == DependencyProperty.UnsetValue)
                    flag = true;
                else if ((bool) obj)
                    return Boxes.BooleanTrue;
            }
            return !flag ? Boxes.BooleanFalse : DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
