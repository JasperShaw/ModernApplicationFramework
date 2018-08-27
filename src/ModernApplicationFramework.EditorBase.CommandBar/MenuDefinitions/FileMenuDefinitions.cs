using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;

namespace ModernApplicationFramework.EditorBase.CommandBar.MenuDefinitions
{
    public static class FileMenuDefinitions
    {
        /*  File --> New    */

        [Export]
        public static CommandBarGroup OpenNewFileGroup =
            new CommandBarGroup(Extended.CommandBarDefinitions.TopLevelMenuDefinitions.FileMenu, 0);

        [Export] public static CommandBarItem NewMenu =
            new CommandBarMenuItem(new Guid("{2EDA6AB7-5780-4E8D-AA3E-193300AC484A}"), CommandBarResources.MenuNew, OpenNewFileGroup, 0);

        [Export] public static CommandBarGroup NewGroup =
            new CommandBarGroup(NewMenu, 0);

        [Export] public static CommandBarItem NewFile =
            new CommandBarCommandItem<NewFileCommandDefinition>(new Guid("{9F65244A-1C98-4BFD-B621-581DE3C0A099}"),
                NewGroup, 2);

        /*  File --> Open   */

        [Export]
        public static CommandBarItem OpenMenu =
            new CommandBarMenuItem(new Guid("{36D45A3A-9A2D-4142-9659-9CC046E2C14F}"), CommandBarResources.MenuOpen, OpenNewFileGroup, 1);

        [Export]
        public static CommandBarGroup OpenFileGroup =
            new CommandBarGroup(OpenMenu, 2);

        [Export]
        public static CommandBarItem OpenFile =
            new CommandBarCommandItem<OpenFileCommandDefinition>(
                new Guid("{0A8C3DE8-A864-4CBD-B2D9-3C97787C85F0}"), OpenFileGroup, 2);

        /*  File --> Save   */

        [Export]
        public static CommandBarGroup SaveGroup =
            new CommandBarGroup(Extended.CommandBarDefinitions.TopLevelMenuDefinitions.FileMenu, 3);

        [Export]
        public static CommandBarItem SaveFile =
            new CommandBarCommandItem<SaveActiveFileCommandDefinition>(
                new Guid("{C68819F7-310A-42AB-A528-344C1C25480B}"), SaveGroup, 0);

        [Export]
        public static CommandBarItem SaveFileAs =
            new CommandBarCommandItem<SaveActiveFileAsCommandDefinition>(
                new Guid("{1B63D36E-E79C-463A-AE10-0526C43A391C}"), SaveGroup, 1);

        [Export]
        public static CommandBarItem SaveAll =
            new CommandBarCommandItem<SaveAllCommandDefinition>(
                new Guid("{0DC3587B-C2E3-4D4B-84F3-BA7F436763A3}"), SaveGroup, 2);

        /*  File --> Recently Used   */

        [Export]
        public static CommandBarGroup RecentlyUsedGroup =
            new CommandBarGroup(Extended.CommandBarDefinitions.TopLevelMenuDefinitions.FileMenu, 6);

        [Export]
        public static CommandBarItem RecentFilesMenu =
            new CommandBarMenuItem(new Guid("{27D0D3B9-DCDE-405D-BCE8-C05ECCEDF74F}"), CommandBarResources.MenuRecentFiles, RecentlyUsedGroup, 0);

        [Export]
        public static CommandBarGroup RecentFilesGroup =
            new CommandBarGroup(RecentFilesMenu, 0);

        [Export]
        public static CommandBarItem SwitchActiveLayoutDocument =
            new CommandBarCommandItem<RecentFilesListDefinition>(
                new Guid("{56C97073-5B9E-46FB-ABD8-635A253F320C}"), RecentFilesGroup, 1);
    }
}