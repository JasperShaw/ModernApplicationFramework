﻿using System;

namespace ModernApplicationFramework.Core.Platform.Enums
{

    [Flags]
    public enum NativeFileFlags : uint
    {
        WriteThrough = 0x80000000,
        Overlapped = 0x40000000,
        NoBuffering = 0x20000000,
        RandomAccess = 0x10000000,
        SequentialScan = 0x8000000,
        DeleteOnClose = 0x4000000,
        BackupSemantics = 0x2000000,
        PosixSemantics = 0x1000000,
        OpenReparsePoint = 0x200000,
        OpenNoRecall = 0x100000
    }
}
