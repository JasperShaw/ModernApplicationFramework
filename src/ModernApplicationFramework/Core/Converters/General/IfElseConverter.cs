using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters.General
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:System.Windows.Data.IValueConverter" /> that boxed a <see langword="bool?"/> to a <see langword="bool"/>
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IValueConverter" />
    public class IfElseConverter : IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? nullable = value as bool?;
            if (value == null)
                throw new ArgumentException("Wrong Format");
            return nullable != null && !nullable.Value ? FalseValue : TrueValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
