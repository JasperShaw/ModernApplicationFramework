using System.Globalization;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal sealed class IsCustomizableModelConverter : ToBooleanValueConverter<CommandBarDataSource>
    {
        protected override bool Convert(CommandBarDataSource selectedItem, object parameter, CultureInfo culture)
        {
            if (selectedItem != null && selectedItem.UiType != CommandControlTypes.Separator)
                if (selectedItem is CommandBarItemDataSource itemDataSource)
                return itemDataSource.IsCustomizable;
            return false;
        }
    }
}
