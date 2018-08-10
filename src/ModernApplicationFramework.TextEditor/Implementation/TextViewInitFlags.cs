using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Flags]
    public enum TextViewInitFlags
    {
        Default = 0,
        Hscroll = 1048576, // 0x00100000
        Vscroll = 2097152, // 0x00200000
        UpdateStatusBar = 4194304, // 0x00400000
        WidgetMargin = 1,
        SelectionMargin = 2,
        VirtualSpace = 4,
        IndentMode = 8,
        StreamSelMode = 16, // 0x00000010
        VisibleWhitespace = 32, // 0x00000020
        Overtype = 64, // 0x00000040
        Dragdropmove = 128, // 0x00000080
        Hoturls = 256, // 0x00000100,
        ProhibitUserInput = 512,
        ChangeTracking = 4096,
        DisableGoBack = 16384,
        Find = 65536
    }
}