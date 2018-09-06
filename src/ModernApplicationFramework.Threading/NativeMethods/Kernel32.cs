﻿using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Threading.NativeMethods
{
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern int WaitForMultipleObjects(uint handleCount, IntPtr[] waitHandles,
            [MarshalAs(UnmanagedType.Bool)] bool waitAll, uint millisecondsTimeout);
    }
}
