using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal sealed class IsCustomizableModelConverter : ToBooleanValueConverter<CommandBarDataSource>
    {
        protected override bool Convert(CommandBarDataSource selectedItem, object parameter, CultureInfo culture)
        {
            if (selectedItem != null && selectedItem.UiType != CommandControlTypes.Separator)
                return selectedItem.IsCustomizable;
            return false;
        }
    }
}
