using System;

namespace ModernApplicationFramework.Native.Platform.Enums
{
    [Flags]
    public enum LoadLibraryExFlags : uint
    {
        DontResolveDllReferences = 0x00000001,
        LoadLibraryAsDatafile = 0x00000002,
        LoadWithAlteredSearchPath = 0x00000008,
        LoadIgnoreCodeAuthzLevel = 0x00000010
    }
}
