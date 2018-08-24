using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;

namespace ModernApplicationFramework.Extended.Demo.Modules.ToolSearch
{
    class TestToolbar
    {
        [Export] public static ToolBarDataSource TestToolBar = new ToolBarDataSource(Guid.NewGuid(), "123", 0, false,Dock.Top, ToolbarScope.Anchorable);

        [Export] public static CommandBarGroup TestGroup = new CommandBarGroup(TestToolBar, 0);

        [Export]
        public static CommandBarItemDataSource Undo =
            new CommandBarCommandItemDataSource<UndoCommandDefinition>(Guid.NewGuid(),
                TestGroup, uint.MinValue);
        [Export]
        public static CommandBarItemDataSource Options =
            new CommandBarCommandItemDataSource<OpenSettingsCommandDefinition>(Guid.NewGuid(), TestGroup, 1);
    }
}
