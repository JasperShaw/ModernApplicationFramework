using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Core.Platform.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Win32StreamId
    {
        public readonly int StreamId;
        public readonly int StreamAttributes;
        public LargeInteger Size;
        public readonly int StreamNameSize;
    }
}
