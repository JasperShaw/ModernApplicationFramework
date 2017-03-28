using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Toolbars
{
    public static class ToolBarDefinitions
    {
        [Export] public static ToolbarDefinition Standard = new ToolbarDefinition("Standard", 0, true, Dock.Top);

        [Export] public static ToolbarItemGroupDefinition StandardUndoRedoGroup = new ToolbarItemGroupDefinition(Standard, 0);

        [Export] public static ToolbarItemDefinition UndoToolBarItem = new CommandToolBarItemDefinition<UndoCommandDefinition>(StandardUndoRedoGroup, 0);

        [Export] public static ToolbarItemDefinition RedoToolBarItem = new CommandToolBarItemDefinition<RedoCommandDefinition>(StandardUndoRedoGroup, 1);
    }
}
