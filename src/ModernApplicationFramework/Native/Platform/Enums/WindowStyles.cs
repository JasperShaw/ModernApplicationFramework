using System;

namespace ModernApplicationFramework.Native.Platform.Enums
{
    [Flags]
    internal enum WindowStyles : uint
    {
        WsBorder = 0x800000,
        WsCaption = 0xc00000,
        WsChild = 0x40000000,
        WsClipchildren = 0x2000000,
        WsClipsiblings = 0x4000000,
        WsDisabled = 0x8000000,
        WsDlgframe = 0x400000,
        WsGroup = 0x20000,
        WsHscroll = 0x100000,
        WsMaximize = 0x1000000,
        WsMaximizebox = 0x10000,
        WsMinimize = 0x20000000,
        WsMinimizebox = 0x20000,
        WsOverlapped = 0x0,
        WsOverlappedwindow = WsOverlapped | WsCaption | WsSysmenu | WsSizeframe | WsMinimizebox | WsMaximizebox,
        WsPopup = 0x80000000u,
        WsPopupwindow = WsPopup | WsBorder | WsSysmenu,
        WsSizeframe = 0x40000,
        WsSysmenu = 0x80000,
        WsTabstop = 0x10000,
        WsVisible = 0x10000000,
        WsVscroll = 0x200000
    }
}
