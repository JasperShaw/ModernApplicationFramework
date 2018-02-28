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
        public static CommandBarGroupDefinition OpenGroup =
            new CommandBarGroupDefinition(OpenMenu, 0);





        [Export]
        public static CommandBarGroupDefinition CloseGroup =
            new CommandBarGroupDefinition(Extended.CommandBarDefinitions.MenuDefinitions.FileMenuDefinitions.FileMenu, 2);

        [Export] public static CommandBarItemDefinition CloseFile =
            new CommandBarCommandItemDefinition<CloseActiveDocumentCommandDefinition>(
                new Guid("{CC88D0C5-163E-480E-A1CE-6A1C748B7807}"), CloseGroup, 0);


    }
}