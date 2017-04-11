using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandMenuItemDefinition : MenuItemDefinition
    {
        public CommandMenuItemDefinition(DefinitionBase commandDefinition, bool isCustom = false, bool isCustomizable = true)
            : base(null, null, uint.MinValue, null, commandDefinition, true, false, isCustom, isCustomizable)
        {
            Text = CommandDefinition?.Text;
        }

        public CommandMenuItemDefinition(DefinitionBase commandDefinition, bool isChecked, bool isCustom = false, bool isCustomizable = true)
            : this(commandDefinition, isCustom, isCustomizable)
        {
            IsChecked = isChecked;
        }
    }
}