using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.Commands;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class ToolsMenuDefinitions
    {
        [Export]
        public static CommandBarGroup SettingsGroup =
            new CommandBarGroup(TopLevelMenuDefinitions.ToolsMenu, int.MaxValue);

        [Export]
        public static CommandBarItem Settings =
            new CommandBarCommandItem<OpenSettingsCommandDefinition>(
                new Guid("{1F0C2B26-D003-437E-949C-D07E4FEF58C9}"), SettingsGroup, int.MaxValue);

        [Export] public static CommandBarItem CustomizeDialog =
            new CommandBarCommandItem<CustomizeMenuCommandDefinition>(
                new Guid("{05B1B0FD-C239-420F-8BC3-A6787D7B2E0A}"), SettingsGroup, int.MaxValue - 1);
    }
}