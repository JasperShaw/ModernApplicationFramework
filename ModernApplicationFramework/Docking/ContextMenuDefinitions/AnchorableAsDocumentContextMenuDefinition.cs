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

        [Export] public static MenuItemGroupDefinition AnchorCloseContextMenuGroup = new MenuItemGroupDefinition(AnchorableAsDocumentContextMenu, uint.MinValue);

        [Export]
        public static MenuItemDefinition CloseCommandItemDefinition =
            new CommandMenuItemDefinition<CloseDockedWindowCommandDefinition>(AnchorCloseContextMenuGroup, 1);

        [Export]
        public static MenuItemDefinition CloseAllButThisCommandItemDefinition =
            new CommandMenuItemDefinition<CloseAllButThisDockedWindowCommandDefinition>(AnchorCloseContextMenuGroup, 2);

        [Export] public static MenuItemGroupDefinition AnchorableContextMenuGroup = new MenuItemGroupDefinition(AnchorableAsDocumentContextMenu, 1);

        [Export]
        public static MenuItemDefinition FloatCommandItemDefinition = new CommandMenuItemDefinition<FloatDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 0);

        [Export] public static MenuItemDefinition DockCommandItemDefinition = new CommandMenuItemDefinition<DockWindowCommandDefinition>(AnchorableContextMenuGroup, 1);

        [Export] public static MenuItemDefinition AutoHideWindowCommandItemDefinition = new CommandMenuItemDefinition<AutoHideWindowCommandDefinition>(AnchorableContextMenuGroup, 2);

        [Export] public static MenuItemDefinition HideCommandItemDefinition = new CommandMenuItemDefinition<HideDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 3);

        [Export] public static MenuItemGroupDefinition DocumentTabGroupContextMenuGroup = new MenuItemGroupDefinition(AnchorableAsDocumentContextMenu, 2);

        [Export]
        public static MenuItemDefinition NewHorizontalTabGroupItemDefinition =
            new CommandMenuItemDefinition<NewHorizontalTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, uint.MinValue, true);

        [Export]
        public static MenuItemDefinition NewVerticalTabGroupItemDefinition =
            new CommandMenuItemDefinition<NewVerticalTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, 1, true);

        [Export]
        public static MenuItemDefinition MoveToNextTabGroupItemDefinition =
            new CommandMenuItemDefinition<MoveToNextTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, 2, true);

        [Export]
        public static MenuItemDefinition MoveAllToNextTabGroupItemDefinition =
            new CommandMenuItemDefinition<MoveAllToNextTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, 3, true);

        [Export]
        public static MenuItemDefinition MoveToPreviousTabGroupItemDefinition =
            new CommandMenuItemDefinition<MoveToPreviousTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, 4, true);

        [Export]
        public static MenuItemDefinition MoveAllToPreviousTabGroupItemDefinition =
            new CommandMenuItemDefinition<MoveAllToPreviousTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, 5, true);

    }
}
