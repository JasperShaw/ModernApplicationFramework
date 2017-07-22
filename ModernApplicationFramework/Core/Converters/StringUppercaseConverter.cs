using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
	public class StringUppercaseConverter : ValueConverter<string, string>
	{
		protected override string Convert(string value, object parameter, CultureInfo culture)
		{
			return value?.ToUpper(culture);
		}
	}
}
