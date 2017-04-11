using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Core.Converters
{
    internal sealed class IsNotSeparatorModelConverter : ToBooleanValueConverter<CommandBarDefinitionBase>
    {
        protected override bool Convert(CommandBarDefinitionBase value, object parameter, CultureInfo culture)
        {
            if (value != null)
                return value.CommandDefinition.ControlType != CommandControlTypes.Separator;
            return false;
        }
    }
}
