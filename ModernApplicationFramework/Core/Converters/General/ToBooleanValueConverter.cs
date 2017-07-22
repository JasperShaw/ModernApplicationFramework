using System;
using System.Globalization;
using System.Windows.Data;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.Converters.General
{
    public class ToBooleanValueConverter<TSource> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TSource) && (value != null || typeof(TSource).IsValueType))
                throw new ArgumentException();
            if (!targetType.IsAssignableFrom(typeof(bool)))
                throw new InvalidOperationException();
            return Boxes.Box(Convert((TSource) value, parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                throw new ArgumentException();
            if (!targetType.IsAssignableFrom(typeof(TSource)))
                throw new InvalidOperationException();
            return ConvertBack((bool) value, parameter, culture);
        }

        protected virtual bool Convert(TSource value, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        protected virtual TSource ConvertBack(bool value, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}