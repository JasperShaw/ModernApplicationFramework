using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    internal class ResizeMenuItemVisiblilityConverter : ValueConverter<CommandBarDefinitionBase, Visibility>
    {
        protected override Visibility Convert(CommandBarDefinitionBase value, object parameter, CultureInfo culture)
        {
            return value == null || value.CommandDefinition.ControlType != CommandControlTypes.Combobox
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
