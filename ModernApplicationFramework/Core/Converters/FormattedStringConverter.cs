using System.Globalization;
using ModernApplicationFramework.Core.Converters.General;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Core.Converters
{
    public class FormattedStringConverter : ValueConverter<string, string>
    {
        protected override string Convert(string value, object parameter, CultureInfo culture)
        {
            var format = CommonUI_Resources.ResourceManager.GetString((string)parameter, CultureInfo.CurrentUICulture);
            return string.Format(culture, format, value);
        }
    }
}
