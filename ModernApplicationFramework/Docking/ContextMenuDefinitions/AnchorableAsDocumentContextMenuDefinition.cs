using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Docking.CommandDefinitions;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public class AnchorableAsDocumentContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition AnchorableAsDocumentContextMenu =
            new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory, ContextMenus_Resources.AnchorableAsDocumentContextMenu_Name);

        [Export] public static CommandBarGroupDefinition AnchorCloseContextMenuGroup =
            new CommandBarGroupDefinition(AnchorableAsDocumentContextMenu, uint.MinValue);

        [Export] public static CommandBarItemDefinition CloseCommandItemDefinition =
            new CommandBarCommandItemDefinition<CloseDockedWindowCommandDefinition>(AnchorCloseContextMenuGroup, 1);

        [Export] public static CommandBarItemDefinition CloseAllButThisCommandItemDefinition =
            new CommandBarCommandItemDefinition<CloseAllButThisDockedWindowCommandDefinition>(
                AnchorCloseContextMenuGroup, 2);

        [Export] public static CommandBarGroupDefinition AnchorableContextMenuGroup =
            new CommandBarGroupDefinition(AnchorableAsDocumentContextMenu, 1);

        [Export] public static CommandBarItemDefinition FloatCommandItemDefinition =
            new CommandBarCommandItemDefinition<FloatDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 0);

        [Export] public static CommandBarItemDefinition DockCommandItemDefinition =
            new CommandBarCommandItemDefinition<DockWindowCommandDefinition>(AnchorableContextMenuGroup, 1);

        [Export] public static CommandBarItemDefinition AutoHideWindowCommandItemDefinition =
            new CommandBarCommandItemDefinition<AutoHideWindowCommandDefinition>(AnchorableContextMenuGroup, 2);

        [Export] public static CommandBarItemDefinition HideCommandItemDefinition =
            new CommandBarCommandItemDefinition<HideDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 3);

        [Export] public static CommandBarGroupDefinition DocumentTabGroupContextMenuGroup =
            new CommandBarGroupDefinition(AnchorableAsDocumentContextMenu, 2);

        [Export] public static CommandBarItemDefinition NewHorizontalTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<NewHorizontalTabGroupCommandDefinition>(
                DocumentTabGroupContextMenuGroup, uint.MinValue);

        [Export] public static CommandBarItemDefinition NewVerticalTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<NewVerticalTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup,
                1);

        [Export] public static CommandBarItemDefinition MoveToNextTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveToNextTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup,
                2);

        [Export] public static CommandBarItemDefinition MoveAllToNextTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveAllToNextTabGroupCommandDefinition>(
                DocumentTabGroupContextMenuGroup, 3);

        [Export] public static CommandBarItemDefinition MoveToPreviousTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveToPreviousTabGroupCommandDefinition>(
                DocumentTabGroupContextMenuGroup, 4);

        [Export] public static CommandBarItemDefinition MoveAllToPreviousTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveAllToPreviousTabGroupCommandDefinition>(
                DocumentTabGroupContextMenuGroup, 5);
    }
}