using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Core.Converters
{
    public class ToolbarItemCommandConverter: ValueConverter<ToolbarItemDefinition, ICommand>
    {
        protected override ICommand Convert(ToolbarItemDefinition value, object parameter, CultureInfo culture)
        {
            return ((CommandDefinition)value.CommandDefinition)?.Command;
        }
    }
}
