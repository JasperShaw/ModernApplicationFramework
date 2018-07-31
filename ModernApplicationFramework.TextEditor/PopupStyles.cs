using System;

namespace ModernApplicationFramework.TextEditor
{
    [Flags]
    public enum PopupStyles
    {
        None = 0,
        DismissOnMouseLeaveText = 4,
        DismissOnMouseLeaveTextOrContent = 8,
        PositionLeftOrRight = 16, // 0x00000010
        PreferLeftOrTopPosition = 32, // 0x00000020
        RightOrBottomJustify = 64, // 0x00000040
        PositionClosest = 128, // 0x00000080
    }
}