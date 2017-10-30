using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.General
{
    /// <summary>
    /// A <see cref="ValueConverter{TSource,TTarget}"/> that returns <see cref="Visibility.Visible"/> if the input was not <see langword="null"/>
    /// </summary>
    public class VisibleIfNotNullConverter : ValueConverter<object, Visibility>
    {
        protected override Visibility Convert(object value, object parameter, CultureInfo culture)
        {
            var visibility = Visibility.Collapsed;
            if (parameter != null)
                visibility = (Visibility) parameter;
            return value != null ? Visibility.Visible : visibility;
        }
    }
}
