using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Converters
{
    internal class VisibleIfAllTrueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Validate.IsNotNull(values, nameof(values));
            var func = (Func<object, bool>)(v =>
            {
                if (v is bool b)
                    return b;
                return false;
            });
            return (Visibility)(values.All(func) ? 0 : 1);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
