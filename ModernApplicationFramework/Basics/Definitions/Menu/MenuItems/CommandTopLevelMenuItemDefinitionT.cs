using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandTopLevelMenuItemDefinition<T> : CommandBarItemDefinition where T : DefinitionBase
    {
        public override DefinitionBase CommandDefinition { get; }

        public CommandTopLevelMenuItemDefinition(CommandBarGroupDefinition group, uint sortOrder,
            bool isCustomizable = true)
            : base(null, sortOrder, group, null, true, true, false, isCustomizable)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;
            Flags.TextOnly = true;
            Flags.Pict = true;
        }
    }
}