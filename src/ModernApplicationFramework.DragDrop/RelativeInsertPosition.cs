﻿using System;

namespace ModernApplicationFramework.DragDrop
{
    [Flags]
    public enum RelativeInsertPosition
    {
        None = 0,
        BeforeTargetItem = 1,
        AfterTargetItem = 2,
        TargetItemCenter = 4
    }
}
