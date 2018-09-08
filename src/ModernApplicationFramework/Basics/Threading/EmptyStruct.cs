using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Basics.Threading
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct EmptyStruct
    {
        internal static EmptyStruct Instance => new EmptyStruct();
    }
}