using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions;

namespace ModernApplicationFramework.EditorBase.CommandBar.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        [Export] public static CommandBarItemDataSource WindowSelectItem=
            new CommandBarCommandItemDataSource<WindowSelectCommandDefinition>(
                new Guid("{B0DE0162-4DCF-409D-AEEB-2F827E93A4A3}"),
                Extended.CommandBar.MenuDefinitions.WindowMenuDefinitions.OpenWindowsGroup, uint.MaxValue);
    }
}
