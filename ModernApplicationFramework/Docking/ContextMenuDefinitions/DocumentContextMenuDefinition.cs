using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Docking.CommandDefinitions;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public static class DocumentContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition DocumentContextMenu =
            new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory, "Easy MDI Document Window");

        [Export] public static CommandBarGroupDefinition DocumentCloseContextMenuGroup =
            new CommandBarGroupDefinition(DocumentContextMenu, uint.MinValue);

        [Export] public static CommandBarItemDefinition CloseCommandItemDefinition =
            new CommandBarCommandItemDefinition<CloseDockedWindowCommandDefinition>(DocumentCloseContextMenuGroup, 1);

        [Export] public static CommandBarItemDefinition CloseAllCommandItemDefinition =
            new CommandBarCommandItemDefinition<CloseAllDockedWindowCommandDefinition>(DocumentCloseContextMenuGroup,
                2);

        [Export] public static CommandBarItemDefinition CloseAllButThisCommandItemDefinition =
            new CommandBarCommandItemDefinition<CloseAllButThisDockedWindowCommandDefinition>(
                DocumentCloseContextMenuGroup, 2);

        [Export] public static CommandBarGroupDefinition DocumentFloatContextMenuGroup =
            new CommandBarGroupDefinition(DocumentContextMenu, 2);

        [Export] public static CommandBarItemDefinition FloatCommandItemDefinition =
            new CommandBarCommandItemDefinition<FloatDockedWindowCommandDefinition>(DocumentFloatContextMenuGroup, 1);

        [Export] public static CommandBarGroupDefinition DocumentTabGroupContextMenuGroup =
            new CommandBarGroupDefinition(DocumentContextMenu, 2);

        [Export] public static CommandBarItemDefinition NewHorizontalTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<NewHorizontalTabGroupCommandDefinition>(
                DocumentTabGroupContextMenuGroup, uint.MinValue, true, false, false, true);

        [Export] public static CommandBarItemDefinition NewVerticalTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<NewVerticalTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup,
                1, true, false, false, true);

        [Export] public static CommandBarItemDefinition MoveToNextTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveToNextTabGroupCommandDefinition>(DocumentTabGroupContextMenuGroup,
                2, true, false, false, true);

        [Export] public static CommandBarItemDefinition MoveAllToNextTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveAllToNextTabGroupCommandDefinition>(
                DocumentTabGroupContextMenuGroup, 3, true, false, false, true);

        [Export] public static CommandBarItemDefinition MoveToPreviousTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveToPreviousTabGroupCommandDefinition>(
                DocumentTabGroupContextMenuGroup, 4, true, false, false, true);

        [Export] public static CommandBarItemDefinition MoveAllToPreviousTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveAllToPreviousTabGroupCommandDefinition>(
                DocumentTabGroupContextMenuGroup, 5, true, false, false, true);
    }
}