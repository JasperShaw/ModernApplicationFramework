using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar.Commands;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class ToolsMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition ToolsMenu =
            new MenuDefinition(new Guid("{F49033D9-C9ED-48C4-B622-2D64FDF7BE41}"),
                MainMenuBarDefinition.MainMenuBarGroup, 8, CommandBar_Resources.MenuTools_Name);

        [Export] public static CommandBarGroupDefinition SettingsGroup =
            new CommandBarGroupDefinition(ToolsMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition CustomizeDialog =
            new CommandBarCommandItemDefinition<CustomizeMenuCommandDefinition>(
                new Guid("{05B1B0FD-C239-420F-8BC3-A6787D7B2E0A}"), SettingsGroup, int.MaxValue - 1);

        [Export] public static CommandBarItemDefinition Settings =
            new CommandBarCommandItemDefinition<OpenSettingsCommandDefinition>(
                new Guid("{1F0C2B26-D003-437E-949C-D07E4FEF58C9}"), SettingsGroup, int.MaxValue);
    }
}