using System.Globalization;
using System.Resources;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    /// <summary>
    /// Abstract <see cref="ValueConverter{TSource,TTarget}"/> that creates a text based on a <see cref="ResourceManager"/>
    /// </summary>
    public abstract class FormattedLocalizableStringConverter : ValueConverter<string, string>
	{
		public abstract ResourceManager ResourceManager { get; }

		protected override string Convert(string value, object parameter, CultureInfo culture)
		{
			var format = ResourceManager.GetString((string)parameter, CultureInfo.CurrentUICulture);
			if (string.IsNullOrEmpty(format))
				return $"The key '{parameter}' needs to be localized";
			return string.Format(culture, format, value);
		}
	}
}