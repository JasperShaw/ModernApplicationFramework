using System.Globalization;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    public class MultiToolTipMultiConverter : MultiValueConverter<string, string, string, string>
    {
        protected override string Convert(string value1, string value2, string value3, object parameter, CultureInfo culture)
        {
            var str = string.IsNullOrEmpty(value1) ? value2 : value1;
            if (string.IsNullOrEmpty(value3))
                return str;
            return string.Format(culture, "{0} ({1})", str, value3);
        }
    }
}
