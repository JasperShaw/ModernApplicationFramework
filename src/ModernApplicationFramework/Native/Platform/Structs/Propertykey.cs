using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Native.Platform.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct Propertykey
    {
        internal Guid fmtid;
        internal uint pid;
    }
}
