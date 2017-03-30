using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Core.Converters
{
    public class CommandBarItemCommandConverter: ValueConverter<CommandBarDefinitionBase, ICommand>
    {
        protected override ICommand Convert(CommandBarDefinitionBase value, object parameter, CultureInfo culture)
        {
            return ((CommandDefinition) value.CommandDefinition)?.Command;
        }
    }
}
