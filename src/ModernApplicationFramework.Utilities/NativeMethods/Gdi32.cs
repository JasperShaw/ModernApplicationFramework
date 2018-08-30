using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Utilities.NativeMethods
{
    internal static class Gdi32
    {
        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
    }
}
