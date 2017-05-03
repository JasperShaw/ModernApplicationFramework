using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.CommandBar.Creators;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.ToolbarDefinition
{
    public static class StandardToolbarDefinition
    {
        [Export] public static Basics.Definitions.Toolbar.ToolbarDefinition Standard = new Basics.Definitions.Toolbar.ToolbarDefinition("Standard", 0, true, Dock.Top);

        [Export] public static CommandBarGroupDefinition StandardUndoRedoGroup = new CommandBarGroupDefinition(Standard, 0);

        [Export] public static CommandBarItemDefinition UndoToolbarItem =
            new CommandBarSplitItemDefinition<MultiUndoCommandDefinition>(
                new NumberStatusStringCreator(Commands_Resources.MultiUndoCommandDefinition_StatusText, Commands_Resources.MultUndoCommandDefinition_StatusPluralSuffix),
                StandardUndoRedoGroup, uint.MinValue);

        [Export] public static CommandBarItemDefinition RedoToolbarItem =
            new CommandBarSplitItemDefinition<MultiRedoCommandDefinition>(
                new NumberStatusStringCreator(Commands_Resources.MultiRedoCommandDefinition_StatusText, Commands_Resources.MultiRedoCommandDefinition_StatusPluralSuffix),
                StandardUndoRedoGroup, 1);
    }
}
