using System.Globalization;

namespace ModernApplicationFramework.Core.Converters.General
{
    public sealed class NotNullConverter : ToBooleanValueConverter<object>
    {
        protected override bool Convert(object value, object parameter, CultureInfo culture)
        {
            return value != null;
        }
    }
}