using System.Globalization;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal sealed class IsSeparatorOrCustomizableModelConverter : ToBooleanValueConverter<CommandBarDataSource>
    {
        protected override bool Convert(CommandBarDataSource value, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value.UiType == CommandControlTypes.Separator)
                return true;
            if (value is CommandBarItemDataSource itemDataSource && itemDataSource.IsCustomizable)
                return true;
            return false;
        }
    }
}
