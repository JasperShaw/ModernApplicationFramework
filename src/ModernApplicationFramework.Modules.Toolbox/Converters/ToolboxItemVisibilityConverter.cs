using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Modules.Toolbox.Converters
{
    public class EmptyMessageVisibilityConverter : MultiValueConverter<bool, bool, bool, Visibility>
    {
        protected override Visibility Convert(bool hasItems, bool hasVisibleItems, bool isExpanded, object parameter, CultureInfo culture)
        {
            if (isExpanded && (!hasVisibleItems || !hasItems))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }
    }
}
