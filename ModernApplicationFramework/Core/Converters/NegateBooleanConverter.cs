using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ModernApplicationFramework.Native.Standard;

namespace ModernApplicationFramework.Core.Converters
{
    public sealed class NegateBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                throw new ArgumentException();
            if(!targetType.IsAssignableFrom(typeof(bool)))
                throw new InvalidOperationException();
            if (!(bool) value)
                return Boxes.BooleanTrue;
            return Boxes.BooleanFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
