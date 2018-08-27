using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBar.Resources;

namespace ModernApplicationFramework.Extended.CommandBar.ToolbarDefinitions
{
	public static class StandardToolbarDefinition
	{
		[Export] public static ToolBarDataSource Standard = new ToolBarDataSource(new Guid("{3E4FEEB6-F0D4-4FCD-B1B9-17BB5384CB0A}"), CommandBar_Resources.ToolBarStandard_Name, uint.MinValue, false, Dock.Top);

		[Export] public static CommandBarGroup StandardUndoRedoGroup = new CommandBarGroup(Standard, 2);

	    [Export] public static CommandBarItem UndoToolbarItem =
	        new CommandBarSplitButton<MultiUndoCommandDefinition>(new Guid("{34B50654-40D7-4322-A22E-4A3CA14E5810}"),
	            StandardUndoRedoGroup, uint.MinValue);

	    [Export]
	    public static CommandBarItem RedoToolbarItem =
	        new CommandBarSplitButton<MultiRedoCommandDefinition>(new Guid("{05843668-186A-43FA-AB89-19DD4DE443E3}"),
	            StandardUndoRedoGroup, 1);


    }
}
