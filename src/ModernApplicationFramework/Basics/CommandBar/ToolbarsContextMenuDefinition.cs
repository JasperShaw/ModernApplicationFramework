using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar.Commands;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;

namespace ModernApplicationFramework.Basics.CommandBar
{
    /// <summary>
    /// Context menu definition for the tool bar tray
    /// </summary>
    public static class ToolbarsContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition ToolbarsContextMenu =
            new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory,
                CommandBarResources.ToolbarsContextMenu_Name);

        [Export] public static CommandBarGroupDefinition ToolBarListGroup =
            new CommandBarGroupDefinition(ToolbarsContextMenu, 0);

        [Export] public static CommandBarItemDefinition ToolBarList =
            new CommandBarCommandItemDefinition<ListToolBarsCommandListDefinition>(ToolBarListGroup, 0);

        [Export] public static CommandBarGroupDefinition CustomizeGroup =
            new CommandBarGroupDefinition(ToolbarsContextMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition Customize =
            new CommandBarCommandItemDefinition<CustomizeMenuCommandDefinition>(CustomizeGroup, 0);
    }
}