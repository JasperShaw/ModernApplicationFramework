using System.Globalization;

namespace ModernApplicationFramework.Core.Converters.General
{
    public sealed class IsNullOrEmptyConverter : ToBooleanValueConverter<string>
    {
        protected override bool Convert(string value, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}
