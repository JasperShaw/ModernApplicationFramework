using System;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{

    /// <summary>
    /// Visual flags for command bar items
    /// </summary>
    [Flags]
    public enum CommandBarFlags
    {
        CommandFlagNone = 0,
        CommandFlagPict = 1,
        CommandFlagText = 2,
        CommandFlagPictAndText = CommandFlagPict | CommandFlagText,
        CommandFlagTextIsAnchor = 4,
        CommandStretchHorizontally = 8,
        CommandFlagFixMenuController = 16,
        CommandFilterKeys = 32,
        CommandFlagComboCommitsOnDrop = 64,
        CommandNoCustomize = 128,
        CommandDefaultInvisible = 256,
        CommandDynamicVisibility = 512
    }
}