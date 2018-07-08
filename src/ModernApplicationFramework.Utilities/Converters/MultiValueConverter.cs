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

    public class MultiValueConverter<TSource1, TSource2, TSource3, TSource4, TTarget> : MultiValueConverterBase<TTarget>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ValidateConvertParameters(values, targetType))
                return default(TTarget);
            return Convert(MultiValueHelper.CheckValue<TSource1>(values, 0), MultiValueHelper.CheckValue<TSource2>(values, 1), MultiValueHelper.CheckValue<TSource3>(values, 2), MultiValueHelper.CheckValue<TSource4>(values, 3), parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            ValidateConvertBackParameters(value, targetTypes);
            MultiValueHelper.CheckType<TSource1>(targetTypes, 0);
            MultiValueHelper.CheckType<TSource2>(targetTypes, 1);
            MultiValueHelper.CheckType<TSource3>(targetTypes, 2);
            MultiValueHelper.CheckType<TSource4>(targetTypes, 3);
            ConvertBack((TTarget)value, out TSource1 obj1, out TSource2 obj2, out TSource3 obj3, out TSource4 obj4, parameter, culture);
            return new object[]
            {
                obj1,
                obj2,
                obj3,
                obj4
            };
        }

        protected virtual TTarget Convert(TSource1 value1, TSource2 value2, TSource3 value3, TSource4 value4, object parameter, CultureInfo culture)
        {
            throw MakeConverterFunctionNotDefinedException();
        }

        protected virtual void ConvertBack(TTarget value, out TSource1 value1, out TSource2 value2, out TSource3 value3, out TSource4 value4, object parameter, CultureInfo culture)
        {
            throw MakeConverterFunctionNotDefinedException();
        }
    }

    public class MultiValueConverter<TSource1, TSource2, TSource3, TSource4, TSource5, TTarget> : MultiValueConverterBase<TTarget>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ValidateConvertParameters(values, targetType))
                return default(TTarget);
            return Convert(MultiValueHelper.CheckValue<TSource1>(values, 0), MultiValueHelper.CheckValue<TSource2>(values, 1), MultiValueHelper.CheckValue<TSource3>(values, 2), MultiValueHelper.CheckValue<TSource4>(values, 3), MultiValueHelper.CheckValue<TSource5>(values, 4), parameter, culture);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            ValidateConvertBackParameters(value, targetTypes);
            MultiValueHelper.CheckType<TSource1>(targetTypes, 0);
            MultiValueHelper.CheckType<TSource2>(targetTypes, 1);
            MultiValueHelper.CheckType<TSource3>(targetTypes, 2);
            MultiValueHelper.CheckType<TSource4>(targetTypes, 3);
            MultiValueHelper.CheckType<TSource5>(targetTypes, 4);
            ConvertBack((TTarget)value, out TSource1 obj1, out TSource2 obj2, out TSource3 obj3, out TSource4 obj4, out TSource5 obj5, parameter, culture);
            return new object[]
            {
                obj1,
                obj2,
                obj3,
                obj4,
                obj5
            };
        }

        protected virtual TTarget Convert(TSource1 value1, TSource2 value2, TSource3 value3, TSource4 value4, TSource5 value5, object parameter, CultureInfo culture)
        {
            throw MakeConverterFunctionNotDefinedException();
        }

        protected virtual void ConvertBack(TTarget value, out TSource1 value1, out TSource2 value2, out TSource3 value3, out TSource4 value4, out TSource5 value5, object parameter, CultureInfo culture)
        {
            throw MakeConverterFunctionNotDefinedException();
        }
    }

    public class MultiValueConverter<T1, T2, T3, T4, T5, T6, T7, T8, T9, TTarget> : MultiValueConverterBase<TTarget>
    {
        public sealed override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ValidateConvertParameters(values, targetType))
                return default(TTarget);
            return Convert(MultiValueHelper.CheckValue<T1>(values, 0), MultiValueHelper.CheckValue<T2>(values, 1), MultiValueHelper.CheckValue<T3>(values, 2), MultiValueHelper.CheckValue<T4>(values, 3), MultiValueHelper.CheckValue<T5>(values, 4), MultiValueHelper.CheckValue<T6>(values, 5), MultiValueHelper.CheckValue<T7>(values, 6), MultiValueHelper.CheckValue<T8>(values, 7), MultiValueHelper.CheckValue<T9>(values, 8), parameter, culture);
        }

        public sealed override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            ValidateConvertBackParameters(value, targetTypes);
            MultiValueHelper.CheckType<T1>(targetTypes, 0);
            MultiValueHelper.CheckType<T2>(targetTypes, 1);
            MultiValueHelper.CheckType<T3>(targetTypes, 2);
            MultiValueHelper.CheckType<T4>(targetTypes, 3);
            MultiValueHelper.CheckType<T5>(targetTypes, 4);
            MultiValueHelper.CheckType<T6>(targetTypes, 5);
            MultiValueHelper.CheckType<T7>(targetTypes, 6);
            MultiValueHelper.CheckType<T8>(targetTypes, 7);
            MultiValueHelper.CheckType<T9>(targetTypes, 8);
            ConvertBack((TTarget)value, out var obj1, out var obj2, out var obj3, out var obj4, out var obj5, out var obj6, out var obj7, out var obj8, out var obj9, parameter, culture);
            return new object[]
            {
                obj1,
                obj2,
                obj3,
                obj4,
                obj5,
                obj6,
                obj7,
                obj8,
                obj9
            };
        }

        protected virtual TTarget Convert(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, object parameter, CultureInfo culture)
        {
            throw MakeConverterFunctionNotDefinedException(nameof(Convert));
        }

        protected virtual void ConvertBack(TTarget value, out T1 value1, out T2 value2, out T3 value3, out T4 value4, out T5 value5, out T6 value6, out T7 value7, out T8 value8, out T9 value9, object parameter, CultureInfo culture)
        {
            throw MakeConverterFunctionNotDefinedException(nameof(ConvertBack));
        }
    }
}