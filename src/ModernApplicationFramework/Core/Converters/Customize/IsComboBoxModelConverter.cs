using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal sealed class IsComboBoxModelConverter : ToBooleanValueConverter<CommandBarDataSource>
    {
        protected override bool Convert(CommandBarDataSource value, object parameter, CultureInfo culture)
        {
            return value?.UiType == CommandControlTypes.Combobox;
        }
    }
}
