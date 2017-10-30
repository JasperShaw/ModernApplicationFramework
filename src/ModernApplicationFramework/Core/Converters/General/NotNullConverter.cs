using System.Globalization;

namespace ModernApplicationFramework.Core.Converters.General
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:System.Windows.Data.IValueConverter" /> that checks if a value is not <see langword="null"/>
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IValueConverter" />
    public sealed class NotNullConverter : ToBooleanValueConverter<object>
    {
        protected override bool Convert(object value, object parameter, CultureInfo culture)
        {
            return value != null;
        }
    }
}