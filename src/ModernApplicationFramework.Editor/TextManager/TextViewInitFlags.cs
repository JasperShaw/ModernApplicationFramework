using System;

namespace ModernApplicationFramework.Editor.TextManager
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
        Readonly = 512,
        ActivateModalState = 1024,
        SuppressStatusBarUpdate = 2048,
        SuppressTrackChanges = 4096,
        SuppressBorder = 8192,
        SuppressTrackGoBack = 16384,
        EnableAutoModusFind = 65536,
        IsEmbeddedView = 131072
    }
}