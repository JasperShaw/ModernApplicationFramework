using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Docking.CommandDefinitions;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public class AnchorableAsDocumentContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition AnchorableAsDocumentContextMenu = new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory, "Easy MDI Tool Window");

        [Export] public static MenuItemGroupDefinition AnchorableAsDocumentContextMenuGroup = new MenuItemGroupDefinition(AnchorableAsDocumentContextMenu, uint.MinValue);
  
        [Export] public static MenuItemDefinition FloatCommandItemDefinition = new CommandMenuItemDefinition<FloatDockedWindowCommandDefinition>(AnchorableAsDocumentContextMenuGroup, 0)
            ;
        [Export] public static MenuItemDefinition DockCommandItemDefinition = new CommandMenuItemDefinition<DockWindowCommandDefinition>(AnchorableAsDocumentContextMenuGroup, 1);

        [Export] public static MenuItemDefinition DockAsTabbedCommandItemDefinition = new CommandMenuItemDefinition<DockAsTabbedDocumentCommandDefinition>(AnchorableAsDocumentContextMenuGroup, 2);

        [Export] public static MenuItemDefinition AutoHideWindowCommandItemDefinition = new CommandMenuItemDefinition<AutoHideWindowCommandDefinition>(AnchorableAsDocumentContextMenuGroup, 3);

        [Export] public static MenuItemDefinition HideCommandItemDefinition = new CommandMenuItemDefinition<HideDockedWindowCommandDefinition>(AnchorableAsDocumentContextMenuGroup, 4);
    }
}
