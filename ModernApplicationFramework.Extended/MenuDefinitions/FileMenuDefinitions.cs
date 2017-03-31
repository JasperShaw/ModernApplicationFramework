using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class FileMenuDefinitions
    {
        [Export] public static MenuDefinition FileMenu = new MenuDefinition(0, "FileMenu", "&File");

        [Export] public static MenuItemGroupDefinition CloseProgramGroup = new MenuItemGroupDefinition(FileMenu, int.MaxValue);

        [Export] public static MenuItemGroupDefinition CloseLayoutItemGroup = new MenuItemGroupDefinition(FileMenu, 3);

        [Export] public static MenuItemDefinition CloseProgram = new CommandMenuItemDefinition<CloseProgammCommandDefinition>(CloseProgramGroup, 1);

        [Export] public static MenuItemDefinition CloseActiveDocument = new CommandMenuItemDefinition<CloseActiveDocumentCommandDefinition>(CloseLayoutItemGroup, 1);
    }
}
