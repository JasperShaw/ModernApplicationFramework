using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    public class EnabledFilteringMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 5)
                throw new ArgumentException("values");
            var flag1 = (bool)values[0];
            var flag2 = (bool)values[2];
            if (flag1 && values[1] != DependencyProperty.UnsetValue)
                return values[1] as IEnumerable;
            if (flag2)
                return values[3] as IEnumerable;
            return values[4] as IEnumerable;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
