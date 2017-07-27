using System.Globalization;

namespace ModernApplicationFramework.Core.Converters.General
{
    public class NullToBooleanConverter : ToBooleanValueConverter<object>
    {
        protected override bool Convert(object value, object parameter, CultureInfo culture)
        {
            return value != null;
        }
    }
}
