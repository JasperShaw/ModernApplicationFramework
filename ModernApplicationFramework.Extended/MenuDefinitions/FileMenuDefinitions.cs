using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class FileMenuDefinitions
    {
        [Export] public static MenuItemDefinition FileMenu = new MenuItemDefinition("_File", 0);

        [Export]
        public static MenuItemDefinition CloseActiveDocument = new MenuItemDefinition<CloseActiveDocumentCommandDefinition>("Close Document", 3, FileMenu);

        [Export]
        public static MenuItemDefinition CloseProgramm = new MenuItemDefinition<CloseProgammCommandDefinition>("Close", int.MaxValue, FileMenu);
    }
}
