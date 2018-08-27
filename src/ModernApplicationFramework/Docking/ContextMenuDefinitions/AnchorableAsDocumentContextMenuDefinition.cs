using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Docking.CommandDefinitions;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public class AnchorableAsDocumentContextMenuDefinition
    {
        [Export] public static ContextMenuDataSource AnchorableAsDocumentContextMenu =
            new ContextMenuDataSource(new Guid("{B3F345F4-65B8-4E5F-A1A8-78A685570595}"), ContextMenuCategory.OtherContextMenusCategory, DockingResources.AnchorableAsDocumentContextMenu_Name);

        [Export] public static CommandBarGroup AnchorCloseContextMenuGroup =
            new CommandBarGroup(AnchorableAsDocumentContextMenu, uint.MinValue);

        [Export] public static CommandBarItem CloseCommandItem =
            new CommandBarCommandItem<CloseDockedWindowCommandDefinition>(new Guid("{997271C3-BFBD-45A7-9F95-CEF05CE42120}"), AnchorCloseContextMenuGroup, 1);

        [Export] public static CommandBarItem CloseAllButThisCommandItem =
            new CommandBarCommandItem<CloseAllButThisDockedWindowCommandDefinition>(new Guid("{B36DE2BA-B399-4190-8FCE-362E55D96EDF}"), 
                AnchorCloseContextMenuGroup, 2);

        [Export] public static CommandBarGroup AnchorableContextMenuGroup =
            new CommandBarGroup(AnchorableAsDocumentContextMenu, 1);

        [Export] public static CommandBarItem FloatCommandItem =
            new CommandBarCommandItem<FloatDockedWindowCommandDefinition>(new Guid("{77890BF4-7C29-4034-88E6-81FF4391F51D}"), AnchorableContextMenuGroup, 0);

        [Export] public static CommandBarItem DockCommandItem =
            new CommandBarCommandItem<DockWindowCommandDefinition>(new Guid("{13C4D0D3-4E98-4EAE-8D18-8D070BA6A6A2}"), AnchorableContextMenuGroup, 1);

        [Export] public static CommandBarItem AutoHideWindowCommandItem =
            new CommandBarCommandItem<AutoHideWindowCommandDefinition>(new Guid("{FCD9A403-2183-4F9D-A807-7CA978499A4C}"), AnchorableContextMenuGroup, 2);

        [Export] public static CommandBarItem HideCommandItem =
            new CommandBarCommandItem<HideDockedWindowCommandDefinition>(new Guid("{CC1D3ACE-D60F-476D-98AD-0CF65F06F2A2}"), AnchorableContextMenuGroup, 3);

        [Export] public static CommandBarGroup DocumentTabGroupContextMenuGroup =
            new CommandBarGroup(AnchorableAsDocumentContextMenu, 2);

        [Export] public static CommandBarItem NewHorizontalTabGroupItemItem =
            new CommandBarCommandItem<NewHorizontalTabGroupCommandDefinition>(new Guid("{3A5197AF-CBB1-4C94-9706-B4A0A5785F7F}"), 
                DocumentTabGroupContextMenuGroup, uint.MinValue);

        [Export] public static CommandBarItem NewVerticalTabGroupItemItem =
            new CommandBarCommandItem<NewVerticalTabGroupCommandDefinition>(new Guid("{7BB57A2A-7CFC-4DA0-AA03-275CD69234CB}"), DocumentTabGroupContextMenuGroup,
                1);

        [Export] public static CommandBarItem MoveToNextTabGroupItemItem =
            new CommandBarCommandItem<MoveToNextTabGroupCommandDefinition>(new Guid("{A243D006-52D9-418B-8DE4-C81BC1582003}"), DocumentTabGroupContextMenuGroup,
                2);

        [Export] public static CommandBarItem MoveAllToNextTabGroupItemItem =
            new CommandBarCommandItem<MoveAllToNextTabGroupCommandDefinition>(new Guid("{A7614D46-A79A-4C39-AA7F-CEC906ED85C4}"), 
                DocumentTabGroupContextMenuGroup, 3);

        [Export] public static CommandBarItem MoveToPreviousTabGroupItemItem =
            new CommandBarCommandItem<MoveToPreviousTabGroupCommandDefinition>(new Guid("{9090390F-C2C4-4E3E-B06D-B81A5C711908}"), 
                DocumentTabGroupContextMenuGroup, 4);

        [Export] public static CommandBarItem MoveAllToPreviousTabGroupItemItem =
            new CommandBarCommandItem<MoveAllToPreviousTabGroupCommandDefinition>(new Guid("{E4FEA66E-F0DC-4E87-A2A9-D1AF405FAD1B}"), 
                DocumentTabGroupContextMenuGroup, 5);
    }
}