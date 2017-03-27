using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.Command;
using DefinitionBase = ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase;

namespace ModernApplicationFramework.Basics.Definitions.Toolbar
{
    public abstract class ToolbarItemDefinition : CommandBarItemDefinitionBase
    {
        public ToolbarItemGroupDefinition Group { get; set; }

        protected ToolbarItemDefinition(ToolbarItemGroupDefinition group, uint sortOrder)
        {
            Group = group;
            SortOrder = sortOrder;
        }

        public sealed override uint SortOrder { get; set; }
    }

    public sealed class CommandToolBarItemDefinition<T> : ToolbarItemDefinition where T : DefinitionBase
    {
        public CommandToolBarItemDefinition(ToolbarItemGroupDefinition group, uint sortOrder) : base(group, sortOrder)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;
        }

        public override string Text { get; set; }

        public override DefinitionBase CommandDefinition { get; set; }
    }
}
