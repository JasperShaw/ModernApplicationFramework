using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.Converters.General
{
    public class AndBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;
            foreach (var obj in values)
            {
                if (obj == DependencyProperty.UnsetValue)
                    flag = true;
                else
                    if (!(bool) obj)
                        return Boxes.BooleanFalse;
            }
            if (!flag)
                return Boxes.BooleanTrue;
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var objArray = new object[1];
            const int index = 0;
            const string str = "ConvertBack";
            objArray[index] = str;
            throw new NotSupportedException();
        }
    }
}