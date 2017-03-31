using System;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    [Flags]
    public enum CommandBarFlags
    {
        CommandFlagPict,
        CommandFlagText,
        CommandFlagPictAndText = CommandFlagPict | CommandFlagText
    }
}
