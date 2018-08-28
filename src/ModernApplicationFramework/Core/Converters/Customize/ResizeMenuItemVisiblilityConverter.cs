using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal sealed class ResizeMenuItemVisiblilityConverter : ValueConverter<CommandBarDataSource, Visibility>
    {
        protected override Visibility Convert(CommandBarDataSource selectedItem, object parameter, CultureInfo culture)
        {
            return !(selectedItem is ComboBoxDataSource) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}