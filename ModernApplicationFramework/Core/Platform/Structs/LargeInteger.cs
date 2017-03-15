using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Core.Platform.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LargeInteger
    {
        public readonly int Low;
        public readonly int High;

        public long ToInt64()
        {
            return (High * 0x100000000) + Low;
        }
    }
}
