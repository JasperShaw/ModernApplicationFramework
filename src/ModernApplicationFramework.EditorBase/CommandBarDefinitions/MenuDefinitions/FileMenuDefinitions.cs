using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.EditorBase.Commands;

namespace ModernApplicationFramework.EditorBase.CommandBarDefinitions.MenuDefinitions
{
    public static class FileMenuDefinitions
    {
        [Export] public static CommandBarGroupDefinition CloseFileGroup =
            new CommandBarGroupDefinition(Extended.CommandBarDefinitions.MenuDefinitions.FileMenuDefinitions.FileMenu, 2);

        [Export] public static CommandBarItemDefinition CloseFile =
            new CommandBarCommandItemDefinition<CloseActiveDocumentCommandDefinition>(
                new Guid("{CC88D0C5-163E-480E-A1CE-6A1C748B7807}"), CloseFileGroup, 0);

        [Export]
        public static CommandBarItemDefinition NewFile =
            new CommandBarCommandItemDefinition<NewFileCommandDefinition>(
                new Guid("{9F65244A-1C98-4BFD-B621-581DE3C0A099}"), CloseFileGroup, 0);
    }
}