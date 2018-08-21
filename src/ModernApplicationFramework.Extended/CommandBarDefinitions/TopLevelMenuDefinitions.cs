using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.CommandBarDefinitions
{
    public static class TopLevelMenuDefinitions
    {
        [Export]
        public static CommandBarItemDataSource FileMenu =
            new MenuDataSource(new Guid("{9A11EF83-5F3E-491C-907B-0F09B982A62D}"),
                MainMenuBarDefinition.MainMenuBarGroup, 0, CommandBar_Resources.MenuFile_Name);

        [Export]
        public static CommandBarItemDataSource EditMenu =
            new MenuDataSource(new Guid("{37DAAE85-3F83-4A99-8AD1-DF2F037C7DD4}"),
                MainMenuBarDefinition.MainMenuBarGroup, 1, CommandBar_Resources.MenuEdit_Name);

        [Export]
        public static CommandBarItemDataSource ViewMenu =
            new MenuDataSource(new Guid("{78420604-4494-454A-8498-51B6DC540539}"),
                MainMenuBarDefinition.MainMenuBarGroup, 2, CommandBar_Resources.MenuView_Name);

        [Export]
        public static CommandBarItemDataSource ToolsMenu =
            new MenuDataSource(new Guid("{F49033D9-C9ED-48C4-B622-2D64FDF7BE41}"),
                MainMenuBarDefinition.MainMenuBarGroup, 8, CommandBar_Resources.MenuTools_Name);

        [Export]
        public static CommandBarItemDataSource WindowMenu =
            new MenuDataSource(new Guid("{A825AD90-0657-45B0-BCA1-1BC8137512BE}"),
                MainMenuBarDefinition.MainMenuBarGroup, 13, CommandBar_Resources.MenuWindow_Name);
    }
}
