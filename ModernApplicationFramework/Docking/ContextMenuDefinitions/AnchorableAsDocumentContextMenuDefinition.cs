using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Docking.CommandDefinitions;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public class AnchorableAsDocumentContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition AnchorableAsDocumentContextMenu = new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory, "Easy MDI Tool Window");

        [Export] public static CommandBarGroupDefinition AnchorCloseContextMenuGroup = new CommandBarGroupDefinition(AnchorableAsDocumentContextMenu, uint.MinValue);

        [Export]
        public static CommandBarItemDefinition CloseCommandItemDefinition =
            new CommandMenuItemDefinition<CloseDockedWindowCommandDefinition>(AnchorCloseContextMenuGroup, 1);

        [Export]
        public static CommandBarItemDefinition CloseAllButThisCommandItemDefinition =
            new CommandMenuItemDefinition<CloseAllButThisDockedWindowCommandDefinition>(AnchorCloseContextMenuGroup, 2);

        [Export] public static CommandBarGroupDefinition AnchorableContextMenuGroup = new CommandBarGroupDefinition(AnchorableAsDocumentContextMenu, 1);

        [Export]
        public static CommandBarItemDefinition FloatCommandItemDefinition = new CommandMenuItemDefinition<FloatDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 0);

        [Export] public static CommandBarItemDefinition DockCommandItemDefinition = new CommandMenuItemDefinition<DockWindowCommandDefinition>(AnchorableContextMenuGroup, 1);

        [Export] public static CommandBarItemDefinition AutoHideWindowCommandItemDefinition = new CommandMenuItemDefinition<AutoHideWindowCommandDefinition>(AnchorableContextMenuGroup, 2);

        [Export] public static CommandBarItemDefinition HideCommandItemDefinition = new CommandMenuItemDefinition<HideDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 3);

        [Export] public static CommandBarGroupDefinition DocumentTabGroupContextMenuGroup = new CommandBarGroupDefinition(AnchorableAsDocumentContextMenu, 2);

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
