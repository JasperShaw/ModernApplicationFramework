using System;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [Flags]
    public enum EnsureSpanVisibleOptions
    {
        ShowStart = 1,
        MinimumScroll = 2,
        AlwaysCenter = 4,
        None = 0
    }
}