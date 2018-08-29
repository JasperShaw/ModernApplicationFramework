using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.Docking.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        //OpenWindowsGroup

        [Export]
        public static CommandBarGroup OpenWindowsGroup =
            new CommandBarGroup(TopLevelMenuDefinitions.WindowMenu, int.MaxValue);

        [Export] public static CommandBarItem SwitchActiveLayoutDocument =
            new CommandBarCommandItem<SwitchToDocumentCommandListDefinition>(
                new Guid("{F87232D2-0BC4-4914-B2C2-2FCE5203C76D}"), OpenWindowsGroup, 0);


        //DocumentToolsGroup

        [Export]
        public static CommandBarGroup DocumentToolsGroup =
            new CommandBarGroup(TopLevelMenuDefinitions.WindowMenu, 4);

        [Export] public static CommandBarItem AutoHideAll =
            new CommandBarCommandItem<AutoHideAllWindowsCommandDefinition>(
                new Guid("{F7F6FE54-2D94-41D9-BACE-CA805EC4719B}"), DocumentToolsGroup, 0);

        [Export] public static CommandBarItem NewHorizontalTabGroupItemItem =
            new CommandBarCommandItem<NewHorizontalTabGroupCommandDefinition>(
                new Guid("{92592EF8-5CAA-48AD-A437-A27874151925}"),
                DocumentToolsGroup, 1, CommandBarFlags.CommandDynamicVisibility);

        [Export] public static CommandBarItem NewVerticalTabGroupItemItem =
            new CommandBarCommandItem<NewVerticalTabGroupCommandDefinition>(
                new Guid("{87242890-3A62-430D-9911-571E0A6B6B51}"), DocumentToolsGroup,
                2, CommandBarFlags.CommandDynamicVisibility);

        [Export] public static CommandBarItem MoveToNextTabGroupItemItem =
            new CommandBarCommandItem<MoveToNextTabGroupCommandDefinition>(
                new Guid("{2C9A9AD3-0F6E-4E11-8023-E10A554B7017}"), DocumentToolsGroup,
                3, CommandBarFlags.CommandDynamicVisibility);

        [Export] public static CommandBarItem MoveAllToNextTabGroupItemItem =
            new CommandBarCommandItem<MoveAllToNextTabGroupCommandDefinition>(
                new Guid("{CEF50FE8-AE34-47AE-B0E1-21256A773E53}"),
                DocumentToolsGroup, 4, CommandBarFlags.CommandDynamicVisibility);

        [Export] public static CommandBarItem MoveToPreviousTabGroupItemItem =
            new CommandBarCommandItem<MoveToPreviousTabGroupCommandDefinition>(
                new Guid("{47098EEF-9B31-4B76-AC76-E519529A2598}"),
                DocumentToolsGroup, 5, CommandBarFlags.CommandDynamicVisibility);

        [Export] public static CommandBarItem MoveAllToPreviousTabGroupItemItem =
            new CommandBarCommandItem<MoveAllToPreviousTabGroupCommandDefinition>(
                new Guid("{B8618F41-9AA8-40E0-8CDD-002BBDE4F119}"),
                DocumentToolsGroup, 6, CommandBarFlags.CommandDynamicVisibility);

        [Export] public static CommandBarItem CloseAllDocuments =
            new CommandBarCommandItem<CloseAllDockedWindowCommandDefinition>(
                new Guid("{72D39275-9E77-4844-BEDB-0237C1B5DE26}"), DocumentToolsGroup, uint.MaxValue);

        //FloatDockGroup
        [Export]
        public static CommandBarGroup FloatDockGroup =
            new CommandBarGroup(TopLevelMenuDefinitions.WindowMenu, 1);

        [Export] public static CommandBarItem Float =
            new CommandBarCommandItem<FloatDockedWindowCommandDefinition>(
                new Guid("{C0EF943D-03AB-4277-9E17-899C888753C7}"), FloatDockGroup, 0);

        [Export] public static CommandBarItem Dock =
            new CommandBarCommandItem<DockWindowCommandDefinition>(
                new Guid("{AEBE128A-2742-4A85-8AAE-9B8D68995019}"), FloatDockGroup, 1);

        [Export] public static CommandBarItem DockAsTabbedDocument =
            new CommandBarCommandItem<DockAsTabbedDocumentCommandDefinition>(
                new Guid("{F6E4024F-863F-443E-BDB8-C577AE01760F}"), FloatDockGroup, 2);

        [Export] public static CommandBarItem AutoHide =
            new CommandBarCommandItem<AutoHideWindowCommandDefinition>(
                new Guid("{B0032BE1-4474-44B7-8C23-8C7BEBF85775}"), FloatDockGroup, 3);

        [Export] public static CommandBarItem Hide =
            new CommandBarCommandItem<HideDockedWindowCommandDefinition>(
                new Guid("{E36D11AA-1882-446F-B917-B181D511015F}"), FloatDockGroup, 4);

        //PinGroup
        [Export]
        public static CommandBarGroup PinGroup =
            new CommandBarGroup(TopLevelMenuDefinitions.WindowMenu, 2);

        [Export]
        public static CommandBarItem Pin =
            new CommandBarCommandItem<PinActiveDocumentCommandDefinition>(
                new Guid("{EB6319CE-F580-4C27-8304-1B97A6C5C44C}"), PinGroup, 0, CommandBarFlags.CommandFlagNone, false, true);

    }
}