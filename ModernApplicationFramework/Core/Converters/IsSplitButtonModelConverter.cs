using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    public class IsSplitButtonModelConverter : ToBooleanValueConverter<CommandBarDefinitionBase>
    {
        protected override bool Convert(CommandBarDefinitionBase value, object parameter, CultureInfo culture)
        {
            return value?.CommandDefinition.ControlType == CommandControlTypes.SplitDropDown;
        }
    }
}
