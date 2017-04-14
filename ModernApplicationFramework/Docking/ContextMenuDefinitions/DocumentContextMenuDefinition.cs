using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Docking.CommandDefinitions;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public static class DocumentContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition DocumentContextMenu = new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory, "Easy MDI Document Window");

        [Export] public static CommandBarGroupDefinition DocumentCloseContextMenuGroup = new CommandBarGroupDefinition(DocumentContextMenu, uint.MinValue);

        [Export] public static CommandBarItemDefinition CloseCommandItemDefinition =
            new CommandMenuItemDefinition<CloseDockedWindowCommandDefinition>(DocumentCloseContextMenuGroup, 1);

        [Export]
        public static CommandBarItemDefinition CloseAllCommandItemDefinition =
            new CommandMenuItemDefinition<CloseAllDockedWindowCommandDefinition>(DocumentCloseContextMenuGroup, 2);

        [Export]
        public static CommandBarItemDefinition CloseAllButThisCommandItemDefinition =
            new CommandMenuItemDefinition<CloseAllButThisDockedWindowCommandDefinition>(DocumentCloseContextMenuGroup, 2);

        [Export] public static CommandBarGroupDefinition DocumentFloatContextMenuGroup = new CommandBarGroupDefinition(DocumentContextMenu, 2);

        [Export]
        public static CommandBarItemDefinition FloatCommandItemDefinition =
            new CommandMenuItemDefinition<FloatDockedWindowCommandDefinition>(DocumentFloatContextMenuGroup, 1);

        [Export] public static CommandBarGroupDefinition DocumentTabGroupContextMenuGroup = new CommandBarGroupDefinition(DocumentContextMenu, 2);

        [Export]
        public static CommandBarItemDefinition NewHorizontalTabGroupItemDefinition =
            new CommandMenuItemDefinition<NewHorizontalTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, uint.MinValue, true);

        [Export]
        public static CommandBarItemDefinition NewVerticalTabGroupItemDefinition =
            new CommandMenuItemDefinition<NewVerticalTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, 1, true);

        [Export]
        public static CommandBarItemDefinition MoveToNextTabGroupItemDefinition =
            new CommandMenuItemDefinition<MoveToNextTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, 2, true);

        [Export]
        public static CommandBarItemDefinition MoveAllToNextTabGroupItemDefinition =
            new CommandMenuItemDefinition<MoveAllToNextTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, 3, true);

        [Export]
        public static CommandBarItemDefinition MoveToPreviousTabGroupItemDefinition =
            new CommandMenuItemDefinition<MoveToPreviousTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, 4, true);

        [Export]
        public static CommandBarItemDefinition MoveAllToPreviousTabGroupItemDefinition =
            new CommandMenuItemDefinition<MoveAllToPreviousTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup, 5, true);
    }
}
