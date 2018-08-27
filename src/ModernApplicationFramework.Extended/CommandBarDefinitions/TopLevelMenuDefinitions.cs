using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.CommandBarDefinitions
{
    public static class TopLevelMenuDefinitions
    {
        [Export] public static CommandBarItem FileMenu =
            new CommandBarMenuItem(new Guid("{9A11EF83-5F3E-491C-907B-0F09B982A62D}"),
                CommandBar_Resources.MenuFile_Name,
                MainMenuBarDefinition.MainMenuBarGroup, 0);

        [Export] public static CommandBarItem EditMenu =
            new CommandBarMenuItem(new Guid("{37DAAE85-3F83-4A99-8AD1-DF2F037C7DD4}"),
                CommandBar_Resources.MenuEdit_Name,
                MainMenuBarDefinition.MainMenuBarGroup, 1);

        [Export] public static CommandBarItem ViewMenu =
            new CommandBarMenuItem(new Guid("{78420604-4494-454A-8498-51B6DC540539}"),
                CommandBar_Resources.MenuView_Name,
                MainMenuBarDefinition.MainMenuBarGroup, 2);

        [Export]
        public static CommandBarItem ToolsMenu =
            new CommandBarMenuItem(new Guid("{F49033D9-C9ED-48C4-B622-2D64FDF7BE41}"), CommandBar_Resources.MenuTools_Name,
                MainMenuBarDefinition.MainMenuBarGroup, 8);

        [Export]
        public static CommandBarItem WindowMenu =
            new CommandBarMenuItem(new Guid("{A825AD90-0657-45B0-BCA1-1BC8137512BE}"), CommandBar_Resources.MenuWindow_Name,
                MainMenuBarDefinition.MainMenuBarGroup, 13);
    }
}
