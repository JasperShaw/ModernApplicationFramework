using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Docking.CommandDefinitions;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class FileMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition FileMenu =
            new MenuDefinition(new Guid("{9A11EF83-5F3E-491C-907B-0F09B982A62D}"),
                MainMenuBarDefinition.MainMenuBarGroup, 0, CommandBar_Resources.MenuFile_Name);

        [Export] public static CommandBarGroupDefinition CloseProgramGroup =
            new CommandBarGroupDefinition(FileMenu, int.MaxValue);

        [Export] public static CommandBarGroupDefinition CloseLayoutItemGroup =
            new CommandBarGroupDefinition(FileMenu, 3);

        [Export] public static CommandBarItemDefinition CloseProgram =
            new CommandBarCommandItemDefinition<CloseProgramCommandDefinition>(
                new Guid("{409F1A03-24DC-4C88-BB23-AED1B18A8167}"), CloseProgramGroup, 1);

        [Export] public static CommandBarItemDefinition CloseActiveDocument =
            new CommandBarCommandItemDefinition<CloseDockedWindowCommandDefinition>(
                new Guid("{6152934A-5610-4DF6-95AD-1D00E489E637}"), CloseLayoutItemGroup, 1);
    }
}