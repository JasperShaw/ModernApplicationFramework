using System.Collections;
using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    public class VisibleIfNotEmptyCollectionConverter : ValueConverter<IEnumerable, Visibility>
    {
        protected override Visibility Convert(IEnumerable value, object parameter, CultureInfo culture)
        {
            return value.Count() != 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
