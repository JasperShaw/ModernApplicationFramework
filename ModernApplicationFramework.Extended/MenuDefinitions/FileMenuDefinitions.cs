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
            new MenuDefinition(MainMenuBarDefinition.MainMenuBarGroup, 0, Menu_Resources.FileMenu_Name);

        [Export] public static CommandBarGroupDefinition CloseProgramGroup =
            new CommandBarGroupDefinition(FileMenu, int.MaxValue);

        [Export] public static CommandBarGroupDefinition CloseLayoutItemGroup =
            new CommandBarGroupDefinition(FileMenu, 3);

        [Export] public static CommandBarItemDefinition CloseProgram =
            new CommandBarCommandItemDefinition<CloseProgammCommandDefinition>(CloseProgramGroup, 1);

        [Export] public static CommandBarItemDefinition CloseActiveDocument =
            new CommandBarCommandItemDefinition<CloseDockedWindowCommandDefinition>(CloseLayoutItemGroup, 1);
    }
}