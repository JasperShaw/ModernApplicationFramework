using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandTopLevelMenuItemDefinition : CommandBarItemDefinition
    {
        public CommandTopLevelMenuItemDefinition(string text, CommandBarGroupDefinition group, uint sortOrder,
            DefinitionBase commandDefinition = null, bool isCustom = false, bool isCustomizable = true)
            : base(text, sortOrder, group, commandDefinition, true, true, isCustom, isCustomizable)
        {
            Flags.TextOnly = true;
            Flags.Pict = true;
        }
    }
}