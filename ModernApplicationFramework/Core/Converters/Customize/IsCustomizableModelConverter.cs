using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters.Customize
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
