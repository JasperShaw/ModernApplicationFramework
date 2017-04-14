using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.Command;
using DefinitionBase = ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandTopLevelMenuItemDefinition<T> : MenuItemDefinition where T : DefinitionBase
    {
        public CommandTopLevelMenuItemDefinition(CommandBarGroupDefinition group, uint sortOrder, bool isCustomizable = true)
            : base(null, null, sortOrder, group, null, true, true, false, isCustomizable)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;
            Flags.TextOnly = true;
            Flags.Pict = true;
        }

        public override DefinitionBase CommandDefinition { get; }
    }
}