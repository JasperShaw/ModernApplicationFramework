using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.Toolbar
{
    public sealed class CommandToolBarItemDefinition<T> : CommandBarItemDefinition where T : DefinitionBase
    {
        public override DefinitionBase CommandDefinition { get; }

        public CommandToolBarItemDefinition(CommandBarGroupDefinition group, uint sortOrder, bool visible = true,
            bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, visible, isChecked, isCustom, isCustomizable)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;
        }
    }
}