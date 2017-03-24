using System.Globalization;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Core.Converters
{
    public class AccessKeyRemovingConverter : ValueConverter<string, string>
    {
        protected override string Convert(string value, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value) ? value : Accelerator.StripAccelerators(value, parameter);
        }
    }
}
