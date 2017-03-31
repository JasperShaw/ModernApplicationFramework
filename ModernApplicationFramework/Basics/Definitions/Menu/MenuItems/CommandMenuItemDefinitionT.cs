using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandMenuItemDefinition<T> : MenuItemDefinition where T : DefinitionBase
    {
        public override DefinitionBase CommandDefinition { get; }

        public CommandMenuItemDefinition(MenuItemGroupDefinition group, uint sortOrder)
            : base(null, sortOrder, group, null, true, false, false)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            DisplayName = CommandDefinition.Text;
        }
    }
}