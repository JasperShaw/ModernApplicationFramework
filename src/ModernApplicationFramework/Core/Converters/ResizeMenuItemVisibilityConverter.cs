using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    internal class ResizeMenuItemVisibilityConverter : ValueConverter<CommandBarDataSource, Visibility>
    {
        protected override Visibility Convert(CommandBarDataSource value, object parameter, CultureInfo culture)
        {
            return value == null || value.UiType != CommandControlTypes.Combobox
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
