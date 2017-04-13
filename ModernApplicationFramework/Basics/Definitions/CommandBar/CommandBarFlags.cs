using System;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    [Flags]
    public enum CommandBarFlags
    {
        CommandFlagNone = 0,
        CommandFlagPict = 1,
        CommandFlagText = 2,
        CommandFlagPictAndText = CommandFlagPict | CommandFlagText
    }
}