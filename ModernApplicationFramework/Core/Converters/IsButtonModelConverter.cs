using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Core.Converters
{
    internal sealed class IsButtonModelConverter : ToBooleanValueConverter<CommandBarDefinitionBase>
    {
        protected override bool Convert(CommandBarDefinitionBase value, object parameter, CultureInfo culture)
        {
            return value?.CommandDefinition.ControlType == CommandControlTypes.Button;
        }
    }
}
