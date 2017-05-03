using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.CommandBar.Creators;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Toolbars
{
    public static class ToolBarDefinitions
    {
        [Export] public static ToolbarDefinition Standard = new ToolbarDefinition("Standard", 0, true, Dock.Top);

        [Export] public static CommandBarGroupDefinition StandardUndoRedoGroup = new CommandBarGroupDefinition(Standard, 0);

        [Export] public static CommandBarItemDefinition UndoToolbarItem = new CommandBarSplitItemDefinition<MultiUndoCommandDefinition>(new NumberStatusStringCreator("Undo {0} Action{1}", "s"), StandardUndoRedoGroup, uint.MinValue);

        [Export] public static CommandBarItemDefinition RedoToolbarItem = new CommandBarSplitItemDefinition<MultiRedoCommandDefinition>(new NumberStatusStringCreator("Redo {0} Action{1}", "s"), StandardUndoRedoGroup, 1);
    }
}
