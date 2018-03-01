using System;

namespace ModernApplicationFramework.Native.Platform.Enums
{
    [Flags]
    public enum FormatMessageFlags
    {
        FormatMessageAllocateBuffer = 0x00000100,
        FormatMessageIgnoreInserts = 0x00000200,
        FormatMessageFromString = 0x00000400,
        FormatMessageFromHmodule = 0x00000800,
        FormatMessageFromSystem = 0x00001000,
        FormatMessageArgumentArray = 0x00002000
    }
}
