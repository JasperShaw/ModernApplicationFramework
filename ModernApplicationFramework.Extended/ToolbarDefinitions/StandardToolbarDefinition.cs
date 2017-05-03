using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.CommandBar.Creators;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.ToolbarDefinitions
{
	public static class StandardToolbarDefinition
	{
		[Export] public static ToolbarDefinition Standard = new ToolbarDefinition(CommandBar_Resources.ToolBarStandard_Name, uint.MinValue, true, Dock.Top);

		[Export] public static CommandBarGroupDefinition StandardUndoRedoGroup = new CommandBarGroupDefinition(Standard, 2);

		[Export] public static CommandBarItemDefinition UndoToolbarItem =
			new CommandBarSplitItemDefinition<MultiUndoCommandDefinition>(
				new NumberStatusStringCreator(Commands_Resources.MultiUndoCommandDefinition_StatusText,
					Commands_Resources.MultiRedoCommandDefinition_StatusSuffix), StandardUndoRedoGroup, uint.MinValue);

		[Export] public static CommandBarItemDefinition RedoToolbarItem =
			new CommandBarSplitItemDefinition<MultiRedoCommandDefinition>(
				new NumberStatusStringCreator(Commands_Resources.MultiRedoCommandDefinition_StatusText,
					Commands_Resources.MultiRedoCommandDefinition_StatusSuffix), StandardUndoRedoGroup, 1);
	}
}
