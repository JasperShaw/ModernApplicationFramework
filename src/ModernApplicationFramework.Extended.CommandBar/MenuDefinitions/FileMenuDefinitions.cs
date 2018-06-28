using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Docking.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class FileMenuDefinitions
    {
        [Export] public static CommandBarGroupDefinition CloseGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.FileMenu, 2);

        [Export] public static CommandBarItemDefinition CloseActiveDocument =
            new CommandBarCommandItemDefinition<CloseDockedWindowCommandDefinition>(
                new Guid("{6152934A-5610-4DF6-95AD-1D00E489E637}"), CloseGroup, 1);


        [Export]
        public static CommandBarGroupDefinition CloseProgramGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.FileMenu, int.MaxValue);

        [Export]
        public static CommandBarItemDefinition CloseProgram =
            new CommandBarCommandItemDefinition<CloseProgramCommandDefinition>(
                new Guid("{409F1A03-24DC-4C88-BB23-AED1B18A8167}"), CloseProgramGroup, 1);
    }
}