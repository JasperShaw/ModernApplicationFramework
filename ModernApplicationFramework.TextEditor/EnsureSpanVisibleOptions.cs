using System;

namespace ModernApplicationFramework.TextEditor
{
    [Flags]
    public enum EnsureSpanVisibleOptions
    {
        ShowStart = 1,
        MinimumScroll = 2,
        AlwaysCenter = 4,
        None = 0,
    }
}