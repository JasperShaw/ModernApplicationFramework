using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandMenuItemDefinition<T> : MenuItemDefinition where T : DefinitionBase
    {
        public CommandMenuItemDefinition(MenuItemGroupDefinition group, uint sortOrder)
            : base(group, sortOrder)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            DisplayName = CommandDefinition.Text;
        }

        public override DefinitionBase CommandDefinition { get; }
    }
}
