using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Core.Converters
{
    internal sealed class IsCustomizableModelConverter : ToBooleanValueConverter<CommandBarDefinitionBase>
    {
        protected override bool Convert(CommandBarDefinitionBase selectedItem, object parameter, CultureInfo culture)
        {
            if (selectedItem != null && selectedItem.CommandDefinition.ControlType != CommandControlTypes.Separator)
                return selectedItem.IsCustomizable;
            return false;
        }
    }
}
