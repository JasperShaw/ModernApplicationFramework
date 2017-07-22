using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    public class ToolTipMultiConverter : MultiValueConverter<string, string, string>
    {
        protected override string Convert(string value1, string value2, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value2))
                return value1;
            return string.Format(culture, "{0} ({1})", value1, value2);
        }
    }
}
