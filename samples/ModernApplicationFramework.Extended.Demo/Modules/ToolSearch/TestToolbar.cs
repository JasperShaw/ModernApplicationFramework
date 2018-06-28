using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.Demo.Modules.ToolSearch
{
    class TestToolbar
    {
        [Export] public static ToolbarDefinition TestToolbarDefinition = new ToolbarDefinition(Guid.NewGuid(), "123", 0, true, Dock.Top, ToolbarScope.Anchorable, false);

        [Export] public static CommandBarGroupDefinition TestGroup = new CommandBarGroupDefinition(TestToolbarDefinition, 0);

        [Export]
        public static CommandBarItemDefinition Undo =
            new CommandBarSplitItemDefinition<UndoCommandDefinition>(Guid.NewGuid(),
                TestGroup, uint.MinValue);
        [Export]
        public static CommandBarItemDefinition Options =
            new CommandBarSplitItemDefinition<OpenSettingsCommandDefinition>(Guid.NewGuid(), TestGroup, 1);
    }
}
