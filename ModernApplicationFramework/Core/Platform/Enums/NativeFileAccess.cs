using System;

namespace ModernApplicationFramework.Core.Platform.Enums
{
    [Flags]
    public enum NativeFileAccess : uint
    {
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000
    }
}
