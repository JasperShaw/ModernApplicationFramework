using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace ModernApplicationFramework.Utilities.Converters
{
    public abstract class MultiValueConverterBase<TTarget> : IMultiValueConverter
    {
        private int _expectedSourceValueCount;

        private int ExpectedSourceValueCount
        {
            get
            {
                if (_expectedSourceValueCount != -1)
                    return _expectedSourceValueCount;
                var type1 = typeof(MultiValueConverterBase<TTarget>);
                var type2 = GetType();
                for (var baseType = type2.BaseType; baseType != type1; baseType = baseType?.BaseType)
                    type2 = baseType;
                _expectedSourceValueCount = type2.GetGenericArguments().Length - 1;
                return _expectedSourceValueCount;
            }
        }

        protected MultiValueConverterBase()
        {
            _expectedSourceValueCount = -1;
        }

        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);

        protected bool ValidateConvertParameters(object[] values, Type targetType)
        {
            if (values.Length != ExpectedSourceValueCount)
                throw new ArgumentException("Not Supported");
            if (values.Any(obj => obj == DependencyProperty.UnsetValue || obj == BindingOperations.DisconnectedSource))
                return false;
            if (!targetType.IsAssignableFrom(typeof(TTarget)))
                throw new InvalidOperationException("Not Supported");
            return true;
        }

        protected void ValidateConvertBackParameters(object value, Type[] targetTypes)
        {
            if (targetTypes.Length != ExpectedSourceValueCount)
                throw new ArgumentException("Not Supported");
            if (!(value is TTarget) && (value != null || typeof(TTarget).IsValueType))
                throw new ArgumentException("Not Supported");
        }

        protected Exception MakeConverterFunctionNotDefinedException([CallerMemberName] string functionName = "")
        {
            return new NotSupportedException("Not Supported");
        }
    }
}
