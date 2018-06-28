using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar.Commands;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class ToolsMenuDefinitions
    {
        [Export]
        public static CommandBarGroupDefinition SettingsGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.ToolsMenu, Int32.MaxValue);

        [Export]
        public static CommandBarItemDefinition Settings =
            new CommandBarCommandItemDefinition<OpenSettingsCommandDefinition>(
                new Guid("{1F0C2B26-D003-437E-949C-D07E4FEF58C9}"), SettingsGroup, Int32.MaxValue);

        [Export] public static CommandBarItemDefinition CustomizeDialog =
            new CommandBarCommandItemDefinition<CustomizeMenuCommandDefinition>(
                new Guid("{05B1B0FD-C239-420F-8BC3-A6787D7B2E0A}"), SettingsGroup, Int32.MaxValue - 1);
    }
}