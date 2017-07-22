using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    public class ClientTextPickerConverter : MultiValueConverter<string, string, string>
    {
        protected override string Convert(string projectClientText, string appIdClientText, object parameter, CultureInfo culture)
        {
            if (projectClientText != null)
                return projectClientText;
            if (appIdClientText == null)
                return string.Empty;
            return appIdClientText;
        }
    }
}
