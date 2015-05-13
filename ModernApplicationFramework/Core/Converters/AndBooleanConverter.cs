using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Core.Platform;

namespace ModernApplicationFramework.Core.Converters
{
    class AndBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            foreach (object obj in values)
            {
                if (obj == DependencyProperty.UnsetValue)
                    flag = true;
                else if (!(bool)obj)
                    return Boxes.BoolFalse;
            }
            if (!flag)
                return Boxes.BoolTrue;
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
