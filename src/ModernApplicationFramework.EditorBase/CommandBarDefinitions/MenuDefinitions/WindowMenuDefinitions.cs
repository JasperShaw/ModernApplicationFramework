using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.EditorBase.Commands;

namespace ModernApplicationFramework.EditorBase.CommandBarDefinitions.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition WindowSelectDefinition=
            new CommandBarCommandItemDefinition<WindowSelectCommand>(
                new Guid("{B0DE0162-4DCF-409D-AEEB-2F827E93A4A3}"),
                Extended.CommandBarDefinitions.MenuDefinitions.WindowMenuDefinitions.OpenWindowsGroup, uint.MaxValue);
    }
}
