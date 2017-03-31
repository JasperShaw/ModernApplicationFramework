using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandMenuItemDefinition : MenuItemDefinition
    {
        public CommandMenuItemDefinition(DefinitionBase commandDefinition, bool isCustom = false)
            : base(commandDefinition, isCustom)
        {
            CommandDefinition = commandDefinition;
            DisplayName = CommandDefinition.Text;
        }

        public override DefinitionBase CommandDefinition { get; }
    }
}
