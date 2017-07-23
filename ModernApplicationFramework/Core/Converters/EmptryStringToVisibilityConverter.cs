using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    /// <summary>
    /// A <see cref="ValueConverter{TSource,TTarget}"/> that converts an empty <see langword="string"/> to <see cref="Visibility.Collapsed"/>
    /// </summary>
    public class EmptryStringToVisibilityConverter  : ValueConverter<string, Visibility>
    {
        protected override Visibility Convert(string value, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
