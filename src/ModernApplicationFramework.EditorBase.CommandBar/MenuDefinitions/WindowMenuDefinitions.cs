using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions;

namespace ModernApplicationFramework.EditorBase.CommandBar.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        [Export] public static CommandBarItem WindowSelectItem =
            new CommandBarCommandItem<WindowSelectCommandDefinition>(
                new Guid("{B0DE0162-4DCF-409D-AEEB-2F827E93A4A3}"),
                Extended.CommandBar.MenuDefinitions.WindowMenuDefinitions.OpenWindowsGroup, uint.MaxValue);
    }
}
