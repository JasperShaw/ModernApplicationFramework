using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Docking.CommandDefinitions;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public class AnchorableContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition AnchorableContextMenu = new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory, "Docked Window");

        [Export] public static CommandBarGroupDefinition AnchorableContextMenuGroup = new CommandBarGroupDefinition(AnchorableContextMenu, uint.MinValue);
  
        [Export] public static MenuItemDefinition FloatCommandItemDefinition = new CommandMenuItemDefinition<FloatDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 0)
            ;
        [Export] public static MenuItemDefinition DockCommandItemDefinition = new CommandMenuItemDefinition<DockWindowCommandDefinition>(AnchorableContextMenuGroup, 1);

        [Export] public static MenuItemDefinition DockAsTabbedCommandItemDefinition = new CommandMenuItemDefinition<DockAsTabbedDocumentCommandDefinition>(AnchorableContextMenuGroup, 2);

        [Export] public static MenuItemDefinition AutoHideWindowCommandItemDefinition = new CommandMenuItemDefinition<AutoHideWindowCommandDefinition>(AnchorableContextMenuGroup, 3);

        [Export] public static MenuItemDefinition HideCommandItemDefinition = new CommandMenuItemDefinition<HideDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 4);
    }
}
