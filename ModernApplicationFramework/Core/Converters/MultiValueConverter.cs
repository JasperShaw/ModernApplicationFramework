using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Core.Converters
{
    public class MultiValueConverter<TSource1, TSource2, TTarget> : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                throw new ArgumentException("Insufficient source parameters: 2");
            if (values.Any(obj => obj == DependencyProperty.UnsetValue || obj == BindingOperations.DisconnectedSource))
            {
                return default (TTarget);
            }
            MultiValueHelper.CheckValue<TSource1>(values, 0);
            MultiValueHelper.CheckValue<TSource2>(values, 1);
            if (!targetType.IsAssignableFrom(typeof(TTarget)))
                throw new InvalidOperationException(string.Format("Target is not from Type: {0}", typeof(TTarget).FullName));
            return Convert((TSource1) values[0], (TSource2) values[1], parameter, culture);
        }

        protected virtual TTarget Convert(TSource1 value1, TSource2 value2, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Convert not defined");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (targetTypes.Length != 2)
                throw new ArgumentException("Insufficient source parameters: 2");
            if (!(value is TTarget) && (value != null || typeof(TTarget).IsValueType))
                throw new ArgumentException(string.Format("Value is not from Type: {0}", typeof(TTarget).FullName));
            MultiValueHelper.CheckType<TSource1>(targetTypes, 0);
            MultiValueHelper.CheckType<TSource2>(targetTypes, 1);
            TSource1 out1;
            TSource2 out2;
            ConvertBack((TTarget) value, out out1, out out2, parameter, culture);
            return new object[]
            {
                out1, out2
            };
        }

        protected virtual void ConvertBack(TTarget value, out TSource1 out1, out TSource2 out2, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack not defined");
        }
    }
}
