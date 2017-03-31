using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.Toolbar
{
    public abstract class ToolbarItemDefinition : CommandBarItemDefinition
    {
        public ToolbarItemGroupDefinition Group { get; set; }

        protected ToolbarItemDefinition(string text, uint sortOrder, DefinitionBase definition, bool visible,
            bool isChecked, bool isCustom, ToolbarItemGroupDefinition group)
            : base(text, sortOrder, definition, visible, isChecked, isCustom)
        {
            Group = group;
        }
    }

    public sealed class CommandToolBarItemDefinition<T> : ToolbarItemDefinition where T : DefinitionBase
    {
        public override DefinitionBase CommandDefinition { get; }

        public CommandToolBarItemDefinition(ToolbarItemGroupDefinition group, uint sortOrder, bool visible = true,
            bool isChecked = false, bool isCustom = false)
            : base(null, sortOrder, null, visible, isChecked, isCustom, group)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;
            IsVisible = visible;
        }
    }
}