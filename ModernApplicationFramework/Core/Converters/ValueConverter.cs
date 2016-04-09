using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters
{
    public class ValueConverter<TSource, TTarget> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == BindingOperations.DisconnectedSource)
                return value;
            if (!(value is TSource) && (value != null || typeof(TSource).IsValueType))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Source is not type: {0}",
                    new object[]
                    {
                        typeof(TSource).FullName
                    }));
            if (targetType.IsAssignableFrom(typeof(TTarget)))
                return Convert((TSource) value, parameter, culture);
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Target is not type: {0}",
                new object[]
                {
                    typeof(TTarget).FullName
                }));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TTarget) && (value != null || typeof(TTarget).IsValueType))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value is not type: {0}",
                    new object[]
                    {
                        typeof(TTarget).FullName
                    }));
            if (targetType.IsAssignableFrom(typeof(TSource)))
                return ConvertBack((TTarget) value, parameter, culture);
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Target is not type: {0}",
                new object[]
                {
                    typeof(TSource).FullName
                }));
        }

        protected virtual TTarget Convert(TSource value, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Converter function is not defined: Convert");
        }

        protected virtual TSource ConvertBack(TTarget value, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Converter function is not defined: ConvertBack");
        }
    }
}