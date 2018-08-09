using System;

namespace ModernApplicationFramework.Utilities.NativeMethods
{
    internal static class NativeMethods
    {
        internal static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size == 4
                ? User32.SetWindowLongPtr32(hWnd, nIndex, dwNewLong)
                : User32.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }
    }
}
