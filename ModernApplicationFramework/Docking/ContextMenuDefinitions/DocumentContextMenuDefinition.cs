using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Docking.CommandDefinitions;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public static class DocumentContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition DocumentContextMenu = new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory, "Easy MDI Document Window");

        [Export] public static MenuItemGroupDefinition DocumentCloseContextMenuGroup = new MenuItemGroupDefinition(DocumentContextMenu, uint.MinValue);

        [Export] public static MenuItemDefinition CloseCommandItemDefinition =
            new CommandMenuItemDefinition<CloseDockedWindowCommandDefinition>(DocumentCloseContextMenuGroup, 1);

        [Export]
        public static MenuItemDefinition CloseAllCommandItemDefinition =
            new CommandMenuItemDefinition<CloseAllDockedWindowCommandDefinition>(DocumentCloseContextMenuGroup, 2);

        [Export]
        public static MenuItemDefinition CloseAllButThisCommandItemDefinition =
            new CommandMenuItemDefinition<CloseAllButThisDockedWindowCommandDefinition>(DocumentCloseContextMenuGroup, 2);

        [Export] public static MenuItemGroupDefinition DocumentFloatContextMenuGroup = new MenuItemGroupDefinition(DocumentContextMenu, 2);

        [Export]
        public static MenuItemDefinition FloatCommandItemDefinition =
            new CommandMenuItemDefinition<FloatDockedWindowCommandDefinition>(DocumentFloatContextMenuGroup, 1);

        [Export] public static MenuItemGroupDefinition DocumentTabGroupContextMenuGroup = new MenuItemGroupDefinition(DocumentContextMenu, 2);

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
