using System;
using System.Globalization;

namespace ModernApplicationFramework.Utilities.Converters
{
    public class MultiValueConverter<TSource1, TSource2, TTarget> : MultiValueConverterBase<TTarget>
    {
        public sealed override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ValidateConvertParameters(values, targetType))
                return default(TTarget);
            return Convert(MultiValueHelper.CheckValue<TSource1>(values, 0), MultiValueHelper.CheckValue<TSource2>(values, 1), parameter, culture);
        }

        public sealed override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            ValidateConvertBackParameters(value, targetTypes);
            MultiValueHelper.CheckType<TSource1>(targetTypes, 0);
            MultiValueHelper.CheckType<TSource2>(targetTypes, 1);
            TSource1 obj1;
            TSource2 obj2;
            ConvertBack((TTarget)value, out obj1, out obj2, parameter, culture);
            return new object[] { obj1, obj2 };
        }

        protected virtual TTarget Convert(TSource1 value1, TSource2 value2, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Convert not defined");
        }

        protected virtual void ConvertBack(TTarget value, out TSource1 out1, out TSource2 out2, object parameter,
                                           CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack not defined");
        }
    }

    public class MultiValueConverter<TSource1, TSource2, TSource3, TTarget> : MultiValueConverterBase<TTarget>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ValidateConvertParameters(values, targetType))
                return default(TTarget);
            return Convert(MultiValueHelper.CheckValue<TSource1>(values, 0), MultiValueHelper.CheckValue<TSource2>(values, 1), MultiValueHelper.CheckValue<TSource3>(values, 2), parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            ValidateConvertBackParameters(value, targetTypes);
            MultiValueHelper.CheckType<TSource1>(targetTypes, 0);
            MultiValueHelper.CheckType<TSource2>(targetTypes, 1);
            MultiValueHelper.CheckType<TSource3>(targetTypes, 2);
            TSource1 obj1;
            TSource2 obj2;
            TSource3 obj3;
            ConvertBack((TTarget)value, out obj1, out obj2, out obj3, parameter, culture);
            return new object[]
            {
                obj1,
                obj2,
                obj3
            };
        }

        protected virtual TTarget Convert(TSource1 value1, TSource2 value2, TSource3 value3, object parameter, CultureInfo culture)
        {
            throw MakeConverterFunctionNotDefinedException();
        }

        protected virtual void ConvertBack(TTarget value, out TSource1 value1, out TSource2 value2, out TSource3 value3, object parameter, CultureInfo culture)
        {
            throw MakeConverterFunctionNotDefinedException();
        }
    }
}