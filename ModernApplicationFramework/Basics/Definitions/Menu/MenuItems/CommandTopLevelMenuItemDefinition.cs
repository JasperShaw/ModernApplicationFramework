using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandTopLevelMenuItemDefinition : MenuItemDefinition
    {
        public CommandTopLevelMenuItemDefinition(string text, MenuItemGroupDefinition group, uint sortOrder, DefinitionBase commandDefinition = null, bool isCustom = false)
            : base(text, group, sortOrder, commandDefinition, isCustom)
        {
            Flags.TextOnly = true;
            Flags.Pict = true;
        }
    }
}
