using System;

namespace ModernApplicationFramework.Extended.Clipboard
{
    [Flags]
    public enum ClipboardPushOption
    {
        Copy = 0,
        Cut = 1,
        Paste = 2,
        Any = Copy | Cut | Paste
    }
}