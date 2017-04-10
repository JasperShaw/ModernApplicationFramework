using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandMenuItemDefinition : MenuItemDefinition
    {
        public CommandMenuItemDefinition(DefinitionBase commandDefinition, bool isCustom = false)
            : base(null, null, uint.MinValue, null, commandDefinition, true, false, isCustom)
        {
            Text = CommandDefinition?.Text;
        }

        public CommandMenuItemDefinition(DefinitionBase commandDefinition, bool isChecked, bool isCustom = false)
            : this(commandDefinition, isCustom)
        {
            IsChecked = isChecked;
        }
    }
}