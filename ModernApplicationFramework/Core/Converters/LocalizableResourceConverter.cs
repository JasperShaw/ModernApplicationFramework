using System;
using System.Globalization;
using System.Resources;
using System.Windows.Data;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Core.Converters
{
	public abstract class LocalizableResourceConverter : IValueConverter
	{
		public abstract ResourceManager ResourceManager { get; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var s = parameter as string;
			if (s == null)
				throw new ArgumentException("The given Parameter is not of Type 'string'");

			var text = CommonUI_Resources.ResourceManager.GetString(s, CultureInfo.CurrentUICulture);
			if (string.IsNullOrEmpty(text))
				return $"The key '{parameter}' needs to be localized";
			return text;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}