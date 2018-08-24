using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Docking.CommandDefinitions;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public static class DocumentContextMenuDefinition
    {
        [Export]
        public static ContextMenuDataSource DocumentContextMenu =
            new ContextMenuDataSource(new Guid("{7EF88CA9-552A-4F28-B850-592FDE415487}"), ContextMenuCategory.OtherContextMenusCategory, DockingResources.DocumentContextMenu_Name);

        [Export]
        public static CommandBarGroup DocumentCloseContextMenuGroup =
            new CommandBarGroup(DocumentContextMenu, uint.MinValue);

        [Export]
        public static CommandBarItemDataSource CloseCommandItem =
            new CommandBarCommandItemDataSource<CloseDockedWindowCommandDefinition>(new Guid("{3A0EEB56-A7DD-4A83-BC7D-6C698CA42532}"), DocumentCloseContextMenuGroup, 1);

        [Export]
        public static CommandBarItemDataSource CloseAllCommandItem =
            new CommandBarCommandItemDataSource<CloseAllDockedWindowCommandDefinition>(new Guid("{FF1BA5F3-E6D0-4C01-8D88-55B696F67178}"), DocumentCloseContextMenuGroup,
                2);

        [Export]
        public static CommandBarItemDataSource CloseAllButThisCommandItem =
            new CommandBarCommandItemDataSource<CloseAllButThisDockedWindowCommandDefinition>(new Guid("{E3F22BE8-7F30-4E69-89F5-F58A023FC5D4}"),
                DocumentCloseContextMenuGroup, 2);

        [Export]
        public static CommandBarGroup DocumentFloatContextMenuGroup =
            new CommandBarGroup(DocumentContextMenu, 2);

        [Export]
        public static CommandBarItemDataSource FloatCommandItem =
            new CommandBarCommandItemDataSource<FloatDockedWindowCommandDefinition>(new Guid("{BB537387-9C07-4CF3-9499-2C4D5DC0ABE0}"), DocumentFloatContextMenuGroup, 1);

        [Export]
        public static CommandBarGroup DocumentTabGroupContextMenuGroup =
            new CommandBarGroup(DocumentContextMenu, 3);

        [Export]
        public static CommandBarItemDataSource NewHorizontalTabGroupItemItem =
            new CommandBarCommandItemDataSource<NewHorizontalTabGroupCommandDefinition>(new Guid("{E960B87A-2E65-4189-A3CC-1E307C841382}"),
                DocumentTabGroupContextMenuGroup, uint.MinValue);

        [Export]
        public static CommandBarItemDataSource NewVerticalTabGroupItemItem =
            new CommandBarCommandItemDataSource<NewVerticalTabGroupCommandDefinition>(new Guid("{21F0EA18-53CE-4075-ADB9-BD0E9DD8D6DD}"), DocumentTabGroupContextMenuGroup,
                1);

        [Export]
        public static CommandBarItemDataSource MoveToNextTabGroupItemItem =
            new CommandBarCommandItemDataSource<MoveToNextTabGroupCommandDefinition>(new Guid("{FEB17F72-D79C-49C7-83D4-607E28CDD7D2}"), DocumentTabGroupContextMenuGroup,
                2);

        [Export]
        public static CommandBarItemDataSource MoveAllToNextTabGroupItemItem =
            new CommandBarCommandItemDataSource<MoveAllToNextTabGroupCommandDefinition>(new Guid("{999E6855-0051-4ACA-95F7-A6B44B50CA4D}"),
                DocumentTabGroupContextMenuGroup, 3);

        [Export]
        public static CommandBarItemDataSource MoveToPreviousTabGroupItemItem =
            new CommandBarCommandItemDataSource<MoveToPreviousTabGroupCommandDefinition>(new Guid("{E4C35551-E7A3-4339-B3C7-C14821566C37}"),
                DocumentTabGroupContextMenuGroup, 4);

        [Export]
        public static CommandBarItemDataSource MoveAllToPreviousTabGroupItemItem =
            new CommandBarCommandItemDataSource<MoveAllToPreviousTabGroupCommandDefinition>(new Guid("{F1B00ABF-19CE-4B21-BCF9-08913ABBC280}"),
                DocumentTabGroupContextMenuGroup, 5);
    }
}