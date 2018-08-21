using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Docking.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        //OpenWindowsGroup

        [Export]
        public static CommandBarGroupDefinition OpenWindowsGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.WindowMenu, int.MaxValue);

        [Export] public static CommandBarItemDataSource SwitchActiveLayoutDocument =
            new CommandBarCommandItemDataSource<SwitchToDocumentCommandListDefinition>(
                new Guid("{F87232D2-0BC4-4914-B2C2-2FCE5203C76D}"), OpenWindowsGroup, 0);


        //DocumentToolsGroup

        [Export]
        public static CommandBarGroupDefinition DocumentToolsGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.WindowMenu, 4);

        [Export] public static CommandBarItemDataSource AutoHideAll =
            new CommandBarCommandItemDataSource<AutoHideAllWindowsCommandDefinition>(
                new Guid("{F7F6FE54-2D94-41D9-BACE-CA805EC4719B}"), DocumentToolsGroup, 0);

        [Export] public static CommandBarItemDataSource NewHorizontalTabGroupItemItem =
            new CommandBarCommandItemDataSource<NewHorizontalTabGroupCommandDefinition>(
                new Guid("{92592EF8-5CAA-48AD-A437-A27874151925}"),
                DocumentToolsGroup, 1, true, false, false, true);

        [Export] public static CommandBarItemDataSource NewVerticalTabGroupItemItem =
            new CommandBarCommandItemDataSource<NewVerticalTabGroupCommandDefinition>(
                new Guid("{87242890-3A62-430D-9911-571E0A6B6B51}"), DocumentToolsGroup,
                2, true, false, false, true);

        [Export] public static CommandBarItemDataSource MoveToNextTabGroupItemItem =
            new CommandBarCommandItemDataSource<MoveToNextTabGroupCommandDefinition>(
                new Guid("{2C9A9AD3-0F6E-4E11-8023-E10A554B7017}"), DocumentToolsGroup,
                3, true, false, false, true);

        [Export] public static CommandBarItemDataSource MoveAllToNextTabGroupItemItem =
            new CommandBarCommandItemDataSource<MoveAllToNextTabGroupCommandDefinition>(
                new Guid("{CEF50FE8-AE34-47AE-B0E1-21256A773E53}"),
                DocumentToolsGroup, 4, true, false, false, true);

        [Export] public static CommandBarItemDataSource MoveToPreviousTabGroupItemItem =
            new CommandBarCommandItemDataSource<MoveToPreviousTabGroupCommandDefinition>(
                new Guid("{47098EEF-9B31-4B76-AC76-E519529A2598}"),
                DocumentToolsGroup, 5, true, false, false, true);

        [Export] public static CommandBarItemDataSource MoveAllToPreviousTabGroupItemItem =
            new CommandBarCommandItemDataSource<MoveAllToPreviousTabGroupCommandDefinition>(
                new Guid("{B8618F41-9AA8-40E0-8CDD-002BBDE4F119}"),
                DocumentToolsGroup, 6, true, false, false, true);

        [Export] public static CommandBarItemDataSource CloseAllDocuments =
            new CommandBarCommandItemDataSource<CloseAllDockedWindowCommandDefinition>(
                new Guid("{72D39275-9E77-4844-BEDB-0237C1B5DE26}"), DocumentToolsGroup, uint.MaxValue);

        //FloatDockGroup
        [Export]
        public static CommandBarGroupDefinition FloatDockGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.WindowMenu, 1);

        [Export] public static CommandBarItemDataSource Float =
            new CommandBarCommandItemDataSource<FloatDockedWindowCommandDefinition>(
                new Guid("{C0EF943D-03AB-4277-9E17-899C888753C7}"), FloatDockGroup, 0);

        [Export] public static CommandBarItemDataSource Dock =
            new CommandBarCommandItemDataSource<DockWindowCommandDefinition>(
                new Guid("{AEBE128A-2742-4A85-8AAE-9B8D68995019}"), FloatDockGroup, 1);

        [Export] public static CommandBarItemDataSource DockAsTabbedDocument =
            new CommandBarCommandItemDataSource<DockAsTabbedDocumentCommandDefinition>(
                new Guid("{F6E4024F-863F-443E-BDB8-C577AE01760F}"), FloatDockGroup, 2, true, false, false, true);

        [Export] public static CommandBarItemDataSource AutoHide =
            new CommandBarCommandItemDataSource<AutoHideWindowCommandDefinition>(
                new Guid("{B0032BE1-4474-44B7-8C23-8C7BEBF85775}"), FloatDockGroup, 3);

        [Export] public static CommandBarItemDataSource Hide =
            new CommandBarCommandItemDataSource<HideDockedWindowCommandDefinition>(
                new Guid("{E36D11AA-1882-446F-B917-B181D511015F}"), FloatDockGroup, 4);


    }
}