using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Modules.Toolbox.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    public static class ToolboxToolbar
    {
        [Export] public static ToolbarDefinition ToolboxToolbarDefinition = new ToolbarDefinition(Guid.NewGuid(), "123", 0, true, Dock.Top, ToolbarScope.Anchorable, false);

        [Export] public static CommandBarGroupDefinition TestGroup = new CommandBarGroupDefinition(ToolboxToolbarDefinition, 0);

        [Export]
        public static CommandBarItemDefinition Add =
            new CommandBarSplitItemDefinition<AddCategoryCommandDefinition>(Guid.NewGuid(),
                TestGroup, uint.MinValue);
        [Export]
        public static CommandBarItemDefinition Remove =
            new CommandBarSplitItemDefinition<DeleteActiveToolbarCategoryCommandDefinition>(Guid.NewGuid(), TestGroup, 1);
    }
}
