using System.Globalization;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.AccessKey
{
    /// <summary>
    /// A <see cref="ValueConverter{TSource,TTarget}"/> that removed access texts
    /// </summary>
    public class AccessKeyRemovingConverter : ValueConverter<string, string>
    {
        protected override string Convert(string value, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value) ? value : Accelerator.StripAccelerators(value, parameter);
        }
    }
}
