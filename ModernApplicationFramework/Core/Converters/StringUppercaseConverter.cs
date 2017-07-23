using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    /// <summary>
    /// A <see cref="ValueConverter{TSource,TTarget}"/> that converts a string to a all upper case string
    /// </summary>
    public class StringUppercaseConverter : ValueConverter<string, string>
	{
		protected override string Convert(string value, object parameter, CultureInfo culture)
		{
			return value?.ToUpper(culture);
		}
	}
}
