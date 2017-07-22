using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    public class ToolTipMultiConverter : MultiValueConverter<string, string, string>
    {
        protected override string Convert(string toolTip, string shortcutText, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(shortcutText))
                return toolTip;
            return string.Format(culture, "{0} ({1})", toolTip, shortcutText);
        }
    }
}
