using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;

namespace ModernApplicationFramework.Extended.Demo.Modules.ToolSearch
{
    class TestToolbar
    {
        [Export] public static CommandBarItem TestToolBar = new CommandBarToolbar(Guid.NewGuid(), "123", 0, 0, false,
            Dock.Top, ToolbarScope.Anchorable, CommandBarFlags.CommandFlagNone);

        [Export] public static CommandBarGroup TestGroup = new CommandBarGroup(TestToolBar, 0);

        [Export]
        public static CommandBarItem Undo =
            new CommandBarCommandItem<UndoCommandDefinition>(Guid.NewGuid(),
                TestGroup, uint.MinValue);
        [Export]
        public static CommandBarItem Options =
            new CommandBarCommandItem<OpenSettingsCommandDefinition>(Guid.NewGuid(), TestGroup, 1);
    }
}
