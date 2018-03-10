using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.EditorBase.Commands;

namespace ModernApplicationFramework.EditorBase.CommandBarDefinitions.MenuDefinitions
{
    public static class FileMenuDefinitions
    {
        [Export]
        public static CommandBarGroupDefinition OpenNewFileGroup =
            new CommandBarGroupDefinition(Extended.CommandBarDefinitions.MenuDefinitions.FileMenuDefinitions.FileMenu, 0);

        [Export] public static CommandBarItemDefinition NewMenu =
            new MenuDefinition(new Guid("{2EDA6AB7-5780-4E8D-AA3E-193300AC484A}"), OpenNewFileGroup, 0, "&New");

        [Export] public static CommandBarGroupDefinition NewGroup =
            new CommandBarGroupDefinition(NewMenu, 0);

        [Export]
        public static CommandBarItemDefinition NewFile =
            new CommandBarCommandItemDefinition<NewFileCommandDefinition>(
                new Guid("{9F65244A-1C98-4BFD-B621-581DE3C0A099}"), NewGroup, 2);


        [Export]
        public static CommandBarItemDefinition OpenMenu =
            new MenuDefinition(new Guid("{36D45A3A-9A2D-4142-9659-9CC046E2C14F}"), OpenNewFileGroup, 1, "&Open");

        [Export]
        public static CommandBarGroupDefinition OpenFileGroup =
            new CommandBarGroupDefinition(OpenMenu, 2);

        [Export]
        public static CommandBarItemDefinition OpenFile =
            new CommandBarCommandItemDefinition<OpenFileCommandDefinition>(
                new Guid("{0A8C3DE8-A864-4CBD-B2D9-3C97787C85F0}"), OpenFileGroup, 2);

        [Export]
        public static CommandBarGroupDefinition SaveGroup =
            new CommandBarGroupDefinition(Extended.CommandBarDefinitions.MenuDefinitions.FileMenuDefinitions.FileMenu, 3);

        [Export]
        public static CommandBarItemDefinition SaveFile =
            new CommandBarCommandItemDefinition<SaveActiveFileCommandDefinition>(
                new Guid("{C68819F7-310A-42AB-A528-344C1C25480B}"), SaveGroup, 0);

        [Export]
        public static CommandBarItemDefinition SaveFileAs =
            new CommandBarCommandItemDefinition<SaveActiveFileAsCommandDefinition>(
                new Guid("{1B63D36E-E79C-463A-AE10-0526C43A391C}"), SaveGroup, 1);

        [Export]
        public static CommandBarItemDefinition SaveAll =
            new CommandBarCommandItemDefinition<SaveAllCommandDefinition>(
                new Guid("{0DC3587B-C2E3-4D4B-84F3-BA7F436763A3}"), SaveGroup, 2);
    }
}