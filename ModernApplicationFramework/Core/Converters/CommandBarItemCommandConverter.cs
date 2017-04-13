using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    public class CommandBarItemCommandConverter: ValueConverter<CommandBarDefinitionBase, ICommand>
    {
        protected override ICommand Convert(CommandBarDefinitionBase value, object parameter, CultureInfo culture)
        {

            var c = value.CommandDefinition as CommandDefinition;

            return c?.Command;
        }
    }
}
