using System;

namespace ModernApplicationFramework.Input.Command
{
    [Flags]
    public enum CommandStatus
    {
        Supported = 1,
        Enabled = 2,
        Checked = 4,
        Invisible = 16,
        HideOnCtxMenu = 32
    }
}