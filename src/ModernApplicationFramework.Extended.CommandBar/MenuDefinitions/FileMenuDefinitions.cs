using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.Docking.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class FileMenuDefinitions
    {
        [Export] public static CommandBarGroup CloseGroup =
            new CommandBarGroup(TopLevelMenuDefinitions.FileMenu, 2);

        [Export] public static CommandBarItem CloseActiveDocument =
            new CommandBarCommandItem<CloseDockedWindowCommandDefinition>(
                new Guid("{6152934A-5610-4DF6-95AD-1D00E489E637}"), CloseGroup, 1);


        [Export]
        public static CommandBarGroup CloseProgramGroup =
            new CommandBarGroup(TopLevelMenuDefinitions.FileMenu, int.MaxValue);

        [Export]
        public static CommandBarItem CloseProgram =
            new CommandBarCommandItem<CloseProgramCommandDefinition>(
                new Guid("{409F1A03-24DC-4C88-BB23-AED1B18A8167}"), CloseProgramGroup, 1);
    }
}