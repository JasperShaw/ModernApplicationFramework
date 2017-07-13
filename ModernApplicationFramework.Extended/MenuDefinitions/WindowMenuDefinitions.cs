using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Docking.CommandDefinitions;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition WindowMenu =
            new MenuDefinition(MainMenuBarDefinition.MainMenuBarGroup, 13, CommandBar_Resources.MenuWindow_Name);

        //OpenWindowsGroup

        [Export] public static CommandBarGroupDefinition OpenWindowsGroup =
            new CommandBarGroupDefinition(WindowMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition SwitchActiveLayoutDocument =
            new CommandBarCommandItemDefinition<SwitchToDocumentCommandListDefinition>(OpenWindowsGroup, 0);

        //LayoutGroup
        [Export]
        public static CommandBarGroupDefinition LayoutGroup =
            new CommandBarGroupDefinition(WindowMenu, 3);

        [Export]
        public static CommandBarItemDefinition SaveLayout =
            new CommandBarCommandItemDefinition<SaveCurrentLayoutCommandDefinition>(LayoutGroup, 0);


        //DocumentToolsGroup

        [Export] public static CommandBarGroupDefinition DocumentToolsGroup =
            new CommandBarGroupDefinition(WindowMenu, 4);

        [Export]
        public static CommandBarItemDefinition AutoHideAll =
            new CommandBarCommandItemDefinition<AutoHideAllWindowsCommandDefinition>(DocumentToolsGroup, 0);

        [Export]
        public static CommandBarItemDefinition NewHorizontalTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<NewHorizontalTabGroupCommandDefinition>(
                DocumentToolsGroup, 1, true, false, false, true);

        [Export]
        public static CommandBarItemDefinition NewVerticalTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<NewVerticalTabGroupCommandDefinition>(DocumentToolsGroup,
                2, true, false, false, true);

        [Export]
        public static CommandBarItemDefinition MoveToNextTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveToNextTabGroupCommandDefinition>(DocumentToolsGroup,
                3, true, false, false, true);

        [Export]
        public static CommandBarItemDefinition MoveAllToNextTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveAllToNextTabGroupCommandDefinition>(
                DocumentToolsGroup, 4, true, false, false, true);

        [Export]
        public static CommandBarItemDefinition MoveToPreviousTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveToPreviousTabGroupCommandDefinition>(
                DocumentToolsGroup, 5, true, false, false, true);

        [Export]
        public static CommandBarItemDefinition MoveAllToPreviousTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveAllToPreviousTabGroupCommandDefinition>(
                DocumentToolsGroup, 6, true, false, false, true);

        [Export]
        public static CommandBarItemDefinition CloseAllDocuments =
            new CommandBarCommandItemDefinition<CloseAllDockedWindowCommandDefinition>(DocumentToolsGroup, uint.MaxValue);

        //FloatDockGroup
        [Export]
        public static CommandBarGroupDefinition FloatDockGroup =
            new CommandBarGroupDefinition(WindowMenu, 1);

        [Export]
        public static CommandBarItemDefinition Float =
            new CommandBarCommandItemDefinition<FloatDockedWindowCommandDefinition>(FloatDockGroup, 0);

        [Export]
        public static CommandBarItemDefinition Dock =
            new CommandBarCommandItemDefinition<DockWindowCommandDefinition>(FloatDockGroup, 1);

        [Export]
        public static CommandBarItemDefinition DockAsTabbedDocument =
            new CommandBarCommandItemDefinition<DockAsTabbedDocumentCommandDefinition>(FloatDockGroup, 2, true, false,false, true);

        [Export]
        public static CommandBarItemDefinition AutoHide =
            new CommandBarCommandItemDefinition<AutoHideWindowCommandDefinition>(FloatDockGroup, 3);

        [Export]
        public static CommandBarItemDefinition Hide =
            new CommandBarCommandItemDefinition<HideDockedWindowCommandDefinition>(FloatDockGroup, 4);


    }
}